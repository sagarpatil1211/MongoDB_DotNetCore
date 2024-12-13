using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using MailKit.Net.Smtp;
using MimeKit;
//using VJ1Core.Infrastructure.Main;

public class EMailTransmitter
{
    private static List<MimeMessage> FormulateEMailMessages(EMailSettings settings, EMailTransmissionRequest request)
    {
        List<MimeMessage> result = new List<MimeMessage>();

        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(settings.SenderFriendlyName, settings.EMailID);
            message.From.Add(from);

            foreach (KeyValuePair<string, string> kvp in request.Receivers)
            {
                string receiverFriendlyName = kvp.Key;
                string receiverEMailID = kvp.Value;

                MailboxAddress to = new MailboxAddress(receiverFriendlyName, receiverEMailID);
                message.To.Add(to);
            }

            message.Subject = request.Subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = request.HTMLBody;
            bodyBuilder.TextBody = request.TextBody;

            foreach (string filePath in request.AttachmentFilePaths)
            {
                bodyBuilder.Attachments.Add(filePath);
            }

            foreach (AttachmentFileStream fStream in request.AttachmentFileStreams)
            {
                bodyBuilder.Attachments.Add(fStream.FileName, fStream.FileStream, fStream.ContentType);
            }

            message.Body = bodyBuilder.ToMessageBody();

            result.Add(message);
        }

        //foreach (KeyValuePair<string, string> kvp in request.Receivers)
        //{
        //    MimeMessage message = new MimeMessage();

        //    MailboxAddress from = new MailboxAddress(settings.SenderFriendlyName, settings.EMailID);
        //    message.From.Add(from);

        //    string receiverFriendlyName = kvp.Key;
        //    string receiverEMailID = kvp.Value;

        //    MailboxAddress to = new MailboxAddress(receiverFriendlyName, receiverEMailID);
        //    message.To.Add(to);

        //    message.Subject = request.Subject;

        //    BodyBuilder bodyBuilder = new BodyBuilder();
        //    bodyBuilder.HtmlBody = request.HTMLBody;
        //    bodyBuilder.TextBody = request.TextBody;

        //    foreach (string filePath in request.AttachmentFilePaths)
        //    {
        //        bodyBuilder.Attachments.Add(filePath);
        //    }

        //    foreach(AttachmentFileStream fStream in request.AttachmentFileStreams)
        //    {
        //        bodyBuilder.Attachments.Add(fStream.FileName, fStream.FileStream, fStream.ContentType);
        //    }

        //    message.Body = bodyBuilder.ToMessageBody();

        //    result.Add(message);
        //}

        return result;
    }

    //private static void SendEMailMessageInternal(EMailSettings settings, MimeMessage message)
    //{
    //    SmtpClient client = new SmtpClient();
    //    client.Connect(settings.SMTPHost, settings.SMTPPort, settings.SSLRequired);
    //    client.Authenticate(settings.UserName, settings.Password);

    //    client.Send(message);
    //    client.Disconnect(true);
    //    client.Dispose();
    //}

    private static MailKit.Security.SecureSocketOptions FormulateSSLConnectionOption(EMailSSLConnectionOptions optn)
    {
        return (MailKit.Security.SecureSocketOptions)Convert.ToInt32(optn);
    }

    public static void SendMultipleMailsBySelectingEMailSettings(DbCommand cmd, List<EMailTransmissionRequest> requests)
    {
        if (requests.Count == 0) return;

        EMailSettings settingsToUse = EMailSettings.GetNextActiveEMailSettings(cmd);

        BackgroundWorker bw = new BackgroundWorker();
        bw.DoWork += (s, ev) =>
        {
            var wrkr = s as BackgroundWorker;

            try
            {
                Action<EMailSettings, MimeMessage> SendEMailMessageInternal = (EMailSettings settings, MimeMessage message) =>
                {
                    SmtpClient client = new SmtpClient();
                    //client.Connect(settings.SMTPHost, settings.SMTPPort, settings.SSLRequired);
                    //client.Connect(settings.SMTPHost, settings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Connect(settings.SMTPHost, settings.SMTPPort, FormulateSSLConnectionOption(settings.SSLConnectionOption));
                    client.Authenticate(settings.UserName, settings.Password);

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                };

                var parameters = ev.Argument as object[];

                EMailSettings ems = parameters[0] as EMailSettings;
                List<EMailTransmissionRequest> lstRequests = parameters[1] as List<EMailTransmissionRequest>;

                foreach (EMailTransmissionRequest request in lstRequests)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

                    var lstMessages = FormulateEMailMessages(ems, request);

                    foreach (MimeMessage message in lstMessages)
                    {
                        try
                        {
                            SendEMailMessageInternal(ems, message);
                        }
                        catch (Exception)
                        {
                            // Swallow for now
                        }
                    }
                }
            }
            catch (Exception)
            {
                wrkr.CancelAsync();
            }
        };

        bw.RunWorkerAsync(new object[] { settingsToUse, requests});
    }

    public static void SendMailBySelectingEMailSettings(DbCommand cmd, EMailTransmissionRequest request)
    {
        //Action<EMailSettings, MimeMessage> SendEMailMessageInternal = (EMailSettings settings, MimeMessage message) =>
        //{
        //    SmtpClient client = new SmtpClient();
        //    client.Connect(settings.SMTPHost, settings.SMTPPort, settings.SSLRequired);
        //    client.Authenticate(settings.UserName, settings.Password);

        //    client.Send(message);
        //    client.Disconnect(true);
        //    client.Dispose();
        //};

        EMailSettings settingsToUse = EMailSettings.GetNextActiveEMailSettings(cmd);

        if (settingsToUse == null) return;

        BackgroundWorker bw = new BackgroundWorker();
        bw.DoWork += (s, ev) =>
        {
            try
            {
                Action<EMailSettings, MimeMessage> SendEMailMessageInternal = (EMailSettings settings, MimeMessage message) =>
                {
                    SmtpClient client = new SmtpClient();
                    //client.Connect(settings.SMTPHost, settings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Connect(settings.SMTPHost, settings.SMTPPort, FormulateSSLConnectionOption(settings.SSLConnectionOption));
                    //client.Connect(settings.SMTPHost, settings.SMTPPort, settings.SSLRequired);
                    client.Authenticate(settings.UserName, settings.Password);

                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                };

                EMailSettings ems = ev.Argument as EMailSettings;

                var lstMessages = FormulateEMailMessages(ems, request);

                foreach (MimeMessage message in lstMessages)
                {
                    try
                    {
                        SendEMailMessageInternal(ems, message);
                    }
                    catch (Exception)
                    {
                        // Swallow for now
                    }
                }
            }
            catch(Exception)
            {
                // Just swallow it for now
            }
        };

        bw.RunWorkerAsync(settingsToUse);

        //while (!sentSuccessfully && loopCount < 2)
        //{
        //    loopCount++;

        //    EMailSettings settingsToUse = EMailSettings.GetNextActiveEMailSettings();

        //    if (settingsToUse != null)
        //    {
        //        try
        //        {
        //            var lstMessages = FormulateEMailMessages(settingsToUse, request);

        //            foreach (MimeMessage message in lstMessages)
        //            {
        //                SendEMailMessageInternal(settingsToUse, message);
        //            }

        //            sentSuccessfully = true;
        //        }
        //        catch(Exception)
        //        {
        //            sentSuccessfully = false;
        //        }
        //    }
        //}
    }

    public static void SendMailSync(EMailSettings settings, EMailTransmissionRequest request)
    {
        if (!settings.EnableEMailTransmission) return;

        var lstMessages = FormulateEMailMessages(settings, request);

        Action<EMailSettings, MimeMessage> SendEMailMessageInternal = (EMailSettings settings, MimeMessage message) =>
        {
            SmtpClient client = new SmtpClient();
            //client.Connect(settings.SMTPHost, settings.SMTPPort, settings.SSLRequired);
            //client.Connect(settings.SMTPHost, settings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
            client.Connect(settings.SMTPHost, settings.SMTPPort, FormulateSSLConnectionOption(settings.SSLConnectionOption));
            client.Authenticate(settings.UserName, settings.Password);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        };

        foreach (MimeMessage message in lstMessages)
        {
            SendEMailMessageInternal(settings, message);
        }
        
    }
}
