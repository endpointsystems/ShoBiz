using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using EndpointSystems.OrchestrationLibrary;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Tracking;
using System.Diagnostics;
using Microsoft.VisualStudio.EFT;
using ThreadState=System.Threading.ThreadState;

namespace EndpointSystems.BizTalk.Documentation
{
    /// <summary>
    /// Orchestration documentation topic.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>{App name}.Orchestrations.{Orchestration Name}</item>
    ///     </list>
    /// </remarks>
    public class OrchestrationTopic : TopicFile
    {
        private readonly string orchName;
        private readonly string imgPath;
        private string imageToken;
        public OrchestrationTopic(string basePath, string btsImgPath, string btsAppName, string btsOrchName)
        {
            try
            {
                path = basePath;
                appName = btsAppName;
                imgPath = btsImgPath;
                tokenId = CleanAndPrep(btsAppName + ".Orchestrations." + btsOrchName);
                TokenFile.GetTokenFile().AddTopicToken(tokenId, id);
                TimerStart();
                orchName = btsOrchName;
                orchWorker_DoWork();
                ReadyToSave = true;
                TimerStop();
            }
            catch(Exception ex)
            {
             PrintLine("[OrchestrationTopic] {0}: {1}",ex.GetType(),ex.Message);   
            }
        }

        private void orchWorker_DoWork()
        {
            BtsCatalogExplorer bce = new BtsCatalogExplorer();
            try
            {
                bce.ConnectionString = CatalogExplorerFactory.CatalogExplorer().ConnectionString;

                BtsOrchestration orch = bce.Applications[appName].Orchestrations[orchName];
                TokenFile.GetTokenFile().AddTopicToken(CleanAndPrep(appName + ".Orchestrations." + orchName), id);
                XElement root = CreateDeveloperConceptualElement();

                root.Add(new XElement(xmlns + "introduction",
                                      new XElement(xmlns + "para",
                                                   new XText(!string.IsNullOrEmpty(orch.Description)
                                                                 ? orch.Description
                                                                 : "No description was available from the orchestration."))));
                Thread t = new Thread(SaveOrchestrationImage);
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                do
                {
                    Trace.WriteLine("waiting to finish saving orchetration image for " + orchName + "...");
                    Thread.Sleep(100);
                } while (Equals(t.ThreadState, ThreadState.Running) || Equals(t.ThreadState, ThreadState.WaitSleepJoin));

                XElement orchSection = new XElement(xmlns + "section");
                XElement content = new XElement(xmlns + "content");

                content.Add(new XElement(xmlns + "token", imageToken), 
                    new XElement(xmlns + "table", new XElement(xmlns + "title", new XText("Orchestration Properties")),
                                                               new XElement(xmlns + "tableHeader",
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Property")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText("Value")))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Application")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XElement(
                                                                                                          xmlns + "token",
                                                                                                          new XText(
                                                                                                              appName)))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Assembly Qualified Name")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XElement(
                                                                                                          xmlns + "token",
                                                                                                          new XText(
                                                                                                              CleanAndPrep
                                                                                                                  (orch.AssemblyQualifiedName))))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Auto Resume Suspended Instances")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          orch.
                                                                                                              AutoResumeSuspendedInstances
                                                                                                              .ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Auto Suspend Running Instances")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          orch.
                                                                                                              AutoSuspendRunningInstances
                                                                                                              .ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Auto Terminate Instances")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          orch.
                                                                                                              AutoTerminateInstances
                                                                                                              .ToString()))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Full Name")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          orch.FullName))),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText("Host")),
                                                                                         GetNullable(orch.Host,"Name",appName + ".Host.")),
                                                                            new XElement(xmlns + "row",
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          "Tracking")),
                                                                                         new XElement(xmlns + "entry",
                                                                                                      new XText(
                                                                                                          orch.Tracking.
                                                                                                              ToString())))));
                orchSection.Add(content);
                root.Add(orchSection);
                List<XElement> portSections = new List<XElement>();
                XElement opsList = new XElement(xmlns + "list", new XAttribute("class", "bullet"));

                foreach (OrchestrationPort port in orch.Ports)
                {
                    if (port.Orchestration.FullName == orchName)
                    {
                        foreach (PortTypeOperation operation in port.PortType.Operations)
                        {
                            opsList.Add(new XElement(xmlns + "listItem",
                                                     new XText(string.Format("{0} ({1})", operation.Name, operation.Type))));
                        }

                        XElement ps = new XElement(xmlns + "section",
                                                   new XElement(xmlns + "title", new XText(port.Name)),
                                                   new XElement(xmlns + "content",
                                                                new XElement(xmlns + "para",
                                                                             new XText(
                                                                                 "This port contains the following properties:")),
                                                                new XElement(xmlns + "table",
                                                                             new XElement(xmlns + "title",
                                                                                          new XText("Port Properties")),
                                                                             new XElement(xmlns + "tableHeader",
                                                                                          new XElement(xmlns + "row",
                                                                                                       new XElement(
                                                                                                           xmlns +
                                                                                                           "entry",
                                                                                                           new XText
                                                                                                               ("Property")),
                                                                                                       new XElement(
                                                                                                           xmlns +
                                                                                                           "entry",
                                                                                                           new XText
                                                                                                               ("Value")))),
                                                                             new XElement(xmlns + "row",
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           "Binding")),
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           port.Binding.
                                                                                                               ToString()))),
                                                                             new XElement(xmlns + "row",
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           "Modifier")),
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           port.Modifier
                                                                                                               .ToString
                                                                                                               ()))),
                                                                             new XElement(xmlns + "row",
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText
                                                                                                           ("Receive Port")),
                                                                                          GetNullable(port.ReceivePort,
                                                                                                      "Name",
                                                                                                      appName +
                                                                                                      ".ReceivePorts.")),
                                                                             new XElement(xmlns + "row",
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           "Send Port")),
                                                                                          GetNullable(port.SendPort,
                                                                                                      "Name",
                                                                                                      appName +
                                                                                                      ".SendPorts.")),
                                                                             new XElement(xmlns + "row",
                                                                                          new XElement(xmlns + "entry",
                                                                                                       new XText(
                                                                                                           "Send Port Group")),
                                                                                          GetNullable(
                                                                                              port.SendPortGroup, "Name",
                                                                                              appName +
                                                                                              ".SendPortGroups."))),
                                                                new XElement(xmlns + "para",new XText("This port definition performs the following operations:")),
                                                                opsList));
                        portSections.Add(ps);
                    }
                }
                root.Add(new XElement(xmlns + "section",
                                      new XElement(xmlns + "title", new XText("Orchestration Ports")),
                                      new XElement(xmlns + "content",
                                                   new XElement(xmlns + "sections", portSections.ToArray()))));

                if (doc.Root != null) doc.Root.Add(root);
            }
            catch(Exception ex)
            {
                PrintLine("{0}: {1}",ex.GetType(),ex.Message);
            }
            finally
            {
                bce.Dispose();
            }
        }

        private void SaveOrchestrationImage()
        {
            lock (this)
            {
                try
            {
                    //make the image token closely match the orchestration token
                    imageToken = tokenId + "Image";
                    string orn = orchName;
                    orn = orn.Replace(" ", string.Empty);
                    orn = orn.Replace(",", string.Empty);
                    //set image ID
                    Guid imgGuid = Guid.NewGuid();
                    //set image path
                    string bmpPath = imgPath + orn + ".jpg";

                    BtsOrch o = new BtsOrch(appName, orchName);
                    using (ODViewCtrl odv = new ODViewCtrl())
                    {
                        odv.AllowDrop = false;
                        odv.BackColor = Color.White;
                        odv.Size = new Size(1024, 768);
                        odv.ShowSchedule(o.ViewData.OuterXml);
                        odv.ResumeLayout(true);
                        odv.Controls.RemoveAt(0);
                        ProcessView pv = (ProcessView) odv.Controls[0].Controls[0];
                        Size s = pv.PreferredSize;
                        odv.Size = s;

                        TokenFile.GetTokenFile().AddImageToken(imageToken, orchName, TokenFile.CaptionPlacement.after,
                                                               imgGuid.ToString(), TokenFile.ImagePlacement.center);
                        ProjectFile.GetProjectFile().AddImageItem(bmpPath, imgGuid.ToString());
                        using (Bitmap bmp = new Bitmap(s.Width, s.Height))
                        {
                            lock (odv)
                            {
                                odv.DrawToBitmap(bmp, new Rectangle(0, 0, s.Width, s.Height));
                                using (FileStream fs = new FileStream(bmpPath, FileMode.Create))
                                {
                                    bmp.Save(fs, ImageFormat.Jpeg);
                                    fs.Close();
                                }
                            }
                        }
                        pv.Dispose();
                    }
                    return;
                }
                catch
                (Exception
                ex)
                {
                    Trace.WriteLine(string.Format("{0} caught saving orchestration {1}: {2} at {3} ", ex.GetType(),
                                                  orchName, ex.Message, ex.StackTrace));
                }
            }
        }

        public XElement GetContentLayout()
        {
            return new XElement("Topic",
                                    new XAttribute("id",id),
                                    new XAttribute("visible","true"),
                                    new XAttribute("title",orchName));
        }
    }
}