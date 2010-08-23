using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.Tracking;
using Microsoft.VisualStudio.EFT;


namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Saves orchestration image files.
    /// </summary>
    public class OrchestrationImage : IObservable<OrchestrationImage>, IDisposable
    {
        private static OrchestrationImage instance;
        private IObserver<OrchestrationImage> _observer;
        private readonly Task SaveTask;

        /// <summary>
        /// Gets or sets the cancellation token for the orchestration image saving process.
        /// </summary>
        public CancellationTokenSource CancelToken;

        /// <summary>
        /// Private implementation - this is a static class.
        /// </summary>
        private OrchestrationImage()
        {            
            Orchestrations = new ConcurrentQueue<BtsOrch>();
            CancelToken = new CancellationTokenSource();
            SaveTask = new Task(SaveOrchestrationImages, CancelToken.Token);
        }
        /// <summary>
        /// Return an instance of the <see cref="OrchestrationImage"/> class.
        /// </summary>
        /// <returns></returns>
        public static OrchestrationImage Instance()
        {
            if (instance == null) instance = new OrchestrationImage();
            return instance;
        }

        /// <summary>
        /// Save the orchestration as an image file to the file system.
        /// </summary>
        public void StartSavingImages()
        {
            SaveTask.RunSynchronously();
        }

        /// <summary>
        /// Indicate that the work for the <see cref="OrchestrationImage"/> instance is finished.
        /// </summary>
        public void Complete()
        {
            if (_observer != null) _observer.OnCompleted();
        }

        /// <summary>
        /// Gets the concurrent queue for the <see cref="BtsOrch"/> objects.
        /// </summary>
        public ConcurrentQueue<BtsOrch> Orchestrations { get; private set; }

        #region IObservable<BtsOrch> Members

        /// <summary>
        /// Subscribes an observer to the observable sequence.
        /// </summary>
        public IDisposable Subscribe(IObserver<OrchestrationImage> observer)
        {
            _observer = observer;
            return this;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            do
            {
                SaveTask.Wait(100);
            } while (!SaveTask.IsCompleted);

            SaveTask.Dispose();
        }

        #endregion

        /// <summary>
        /// Do the actual work of saving orchestration images.
        /// </summary>
        [STAThread]
        private void SaveOrchestrationImages()
        {
            do
            {
                if (Orchestrations.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    BtsOrch orch;
                    if (!Orchestrations.TryDequeue(out orch)) continue;
                    if (orch == null) continue;

                    // notify the observer that we're working on the next image

                    if (_observer != null) _observer.OnNext(this);

                    if (string.IsNullOrEmpty(orch.ApplicationName))
                        throw new NullReferenceException("Orchestration " + orch.Name + " has no BizTalk application assigned to it.");

                    if (string.IsNullOrEmpty(ProjectConfiguration.ImagesPath))
                        throw new NullReferenceException("ProjectConfiguration contains a null or empty image path.");

                    //make the image token closely match the orchestration token
                    var tokenId = CleanAndPrep(orch.ApplicationName + ".Orchestrations." + orch.Name);

                    var imageToken = tokenId + "Image";
                    var orn = orch.Name.Replace(" ",string.Empty).Replace(",",string.Empty);
                    
                    //set image ID
                    var imgGuid = Guid.NewGuid();

                    //set image path
                    var bmpFileName = orn + ".jpg";
                    var bmpPath = Path.Combine(ProjectConfiguration.ImagesPath, bmpFileName);

                    //save the orchestration bitmap
                    using (var odv = new ODViewCtrl())
                    {
                        odv.AllowDrop = false;
                        odv.BackColor = Color.White;
                        odv.Size = new Size(1024, 768);
                        odv.ShowSchedule(orch.ViewData.OuterXml);
                        odv.ResumeLayout(true);
                        odv.Controls.RemoveAt(0);
                        using (var pv = (ProcessView)odv.Controls[0].Controls[0])
                        {
                            var s = pv.PreferredSize;
                            odv.Size = s;

                            using (var bmp = new Bitmap(s.Width, s.Height))
                            {
                                lock (odv)
                                {
                                    odv.DrawToBitmap(bmp, new Rectangle(0, 0, s.Width, s.Height));
                                    using (var fs = new FileStream(bmpPath, FileMode.Create))
                                    {
                                        
                                        bmp.Save(fs, ImageFormat.Jpeg);
                                        fs.Close();
                                    }
                                }
                            }
                        }
                    }

                    //Save the new orchestration image info to the project
                    TokenFile.GetTokenFile().AddImageToken(imageToken, orch.Name, TokenFile.CaptionPlacement.after,
                                                           imgGuid.ToString(), TokenFile.ImagePlacement.center);
                    ProjectFile.GetProjectFile().AddImageItem(Path.Combine("images",bmpFileName), imgGuid.ToString());
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }


            } while (!CancelToken.IsCancellationRequested && Orchestrations.Count > 0);
        }


        private void HandleException(Exception ex)
        {
            PrintLine("[OrchestrationImage] {0}: {1}\r\n{2}",ex.GetType(),ex.Message,ex.StackTrace);
            _observer.OnError(ex);
        }

        /// <summary>
        /// Cleanup a qualified name, removing non-alphanumeric characters.
        /// </summary>
        /// <param name="qualifiedName">The qualified name to clean.</param>
        /// <returns>A string with only alphanumeric characters.</returns>
        private string CleanAndPrep(string qualifiedName)
        {
            try
            {
                if (string.IsNullOrEmpty(qualifiedName))
                {
                    Trace.WriteLine("CleanAndPrep got a null/empty string!");
                    return string.Empty;
                }
                var s = qualifiedName.Replace(",", ".");
                s = s.Replace("=", "-");
                s = s.Replace("#", "-");
                return s.Replace(" ", "");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// Perform a trace operation.
        /// </summary>
        /// <param name="message">The format message.</param>
        /// <param name="args">The optional format arguments.</param>
        protected static void PrintLine(string message, params object[] args)
        {
            if (args == null || args.Length < 1)
            {
                Trace.WriteLine(message);
                return;
            }
            Trace.WriteLine(string.Format(message, args));
        }
    }
}
