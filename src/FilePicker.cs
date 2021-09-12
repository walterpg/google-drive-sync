using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.Plugins;
using Newtonsoft.Json;

namespace KPSyncForDrive
{
    class FilePicker
    {
        readonly SyncConfiguration m_syncConfig;
        readonly IPluginHost m_host;
        readonly DatabaseContext m_dbCtx;

        // TODO: Add multiple options since these have to be added as authorized origins on Google API side.
        static int[] m_allowedPorts = new int[] { 50100 };

        private static int GetRandomAllowedPort()
        {
            var r = new Random();
            return m_allowedPorts[r.Next(m_allowedPorts.Length)];
        }

        internal FilePicker(IPluginHost host, DatabaseContext dbCtx,
            SyncConfiguration syncConfig)
        {
            m_syncConfig = syncConfig;
            m_host = host;
            m_dbCtx = dbCtx;
        }

        public async Task<FilePick> SelectFile(CancellationToken cancellationToken)
        {
            FilePick pick;
            using (AuthWaitOrCancel form = new AuthWaitOrCancel(m_host, m_dbCtx, m_syncConfig as EntryConfiguration))
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    var httpHandlerTask = HandleRequests(
                        cts.Token,
                        (uri) =>
                        {
                            Log.Debug("Opening OS URL handler ('{0}').", uri);
                            Process.Start(uri);
                        },
                        () =>
                        {
                            if (!form.IsDisposed)
                            {
                                Log.Debug("Attempting to close waiter dialog.");
                                form.Invoke(new MethodInvoker(form.Close));
                            }
                        }
                    );

                    // Wait for dialog.
                    form.FormClosing += (o, e) => { Log.Debug("Closing"); if (!cts.IsCancellationRequested) { cts.Cancel(); } };

                    DialogResult dr = KPSyncForDriveExt.ShowModalDialogAndDestroy(form);

                    pick = await httpHandlerTask;
                    cts.Cancel();

                    Log.Debug("Wait dialog returned '{0}'.", dr.ToString("G"));
                }
            }

            // Bring main KeePass window back.
            Log.Debug("Activating KP main window.");
            Form window = m_host.MainWindow;
            window.BeginInvoke(new MethodInvoker(window.Activate));

            return pick;
        }

        private async Task<FilePick> HandleRequests(CancellationToken cancellationToken, Action<string> onListenerStart, Action onPicked)
        {
            var uri = string.Format("{0}:{1}/", "http://localhost", GetRandomAllowedPort());

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(uri);
                try
                {
                    listener.Start();
                }
                catch (HttpListenerException ex)
                {
                    Log.Error("Failed to start HttpListener with error code: {0}", ex.ErrorCode);
                    return null;
                }

                Log.Debug("Listening for '{0}'...", uri);

                onListenerStart(uri);

                var pageBuffer = Resources.ReadByteResourceFromAssembly("Pages.picker.html");

                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(TimeSpan.FromMinutes(5)); // Give 5 minutes or otherwise cancel.
                    cts.Token.Register(() => listener.Stop());

                    while (!cts.Token.IsCancellationRequested)
                    {
                        HttpListenerContext context = null;
                        try
                        {
                            context = await listener.GetContextAsync();
                        }
                        catch (HttpListenerException ex)
                        {
                            Log.Error("Failed to get context: {0}", ex.ErrorCode);
                        }

                        if (context == null)
                        {
                            Log.Debug("Assuming user cancelled the listener.");
                            break;
                        }

                        if (context.Request.HttpMethod == "POST")
                        {
                            using (var sr = new StreamReader(context.Request.InputStream))
                            {
                                var input = await sr.ReadToEndAsync();
                                FilePick pick = null;
                                try
                                {
                                    pick = JsonConvert.DeserializeObject<FilePick>(input);
                                    Log.Debug("Picked {0}, {1}", pick.Id, pick.Name);
                                    UpdateSyncConfiguration(pick);
                                }
                                catch (JsonSerializationException ex)
                                {
                                    Log.Error(ex, "Failed to deserialize file pick response");
                                }

                                var response = context.Response;
                                response.StatusCode = 200;
                                response.Close();

                                Log.Debug("Returning picked {0}", pick.Id);
                                onPicked();
                                return pick;
                            }
                        }
                        else
                        {
                            var response = context.Response;
                            response.KeepAlive = false;
                            response.ContentLength64 = pageBuffer.Length;
                            using (Stream responseOutput = response.OutputStream)
                            {
                                await responseOutput.WriteAsync(pageBuffer, 0, pageBuffer.Length);
                                Log.Debug("File picker page returned to browser.");
                            }
                        }
                    }
                }
            }

            onPicked();
            return null;
        }

        private void UpdateSyncConfiguration(FilePick pick)
        {
            m_syncConfig.FileScopeFileTarget = pick.Name;
            m_syncConfig.FileScopeFileTargetId = pick.Id;
        }
    }

    internal class FilePick
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
