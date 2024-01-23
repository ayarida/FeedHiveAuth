using FeedHiveAuth.Areas.Social.SocialFacebook;
using FeedHiveAuth.Areas.Social.SocialWhatsapp.Models;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialWhatsapp.Clients
{
    public class WhatsappClient
    {
        private WhatsappCredentials Credentials { get; set; }
        private readonly RestClientService Service;

        public class AjaxData
        {
            public string op { get; set; }
            public string bbs { get; set; }
            public string nickname { get; set; }
            public Dictionary<string, dynamic> data { get; set; }
        }

        public WhatsappClient(WhatsappCredentials credentials)
        {
            Credentials = credentials;
            Service = new RestClientService("https://my.massejli.com/Programs/massejli/ajax/ajax_api.php");
        }
        public string SendMessage(string audioURL, string documentURL, string videoURL, string imageURL, string link, string text, string title, string youtubeLink, bool sendToGroups, bool sendToContacts, bool setCaption, bool useTemplate)
        {


            var message = new WhatsappViewModel()
            {
                Audio = audioURL,
                Document = documentURL,
                Link = link,
                Message = text,
                Photo = imageURL,
                Title = title,
                Video = videoURL,
                Youtube = youtubeLink,
                SendToContacts = sendToContacts,
                SendToGroups = sendToGroups,
                SetCaption = setCaption,
                UseTemplate = useTemplate,

            };

            var result = SendPush(message);
            return result;
        }
        private string SendPush(WhatsappViewModel data)
        {

            var client = new RestClient("https://my.massejli.com/Programs/massejli/ajax/ajax_api.php");
            var request = new RestRequest(Method.POST);
            string serviceNumbersInput = Credentials.serviceNumber;
            string[] serviceNumbers = new String[serviceNumbersInput.Split(',').Length];
            serviceNumbers = serviceNumbersInput.Split(',');

            AjaxData _AjaxData = new AjaxData();
            _AjaxData.op = "publish_message";
            _AjaxData.bbs = Credentials.bbs;
            _AjaxData.nickname = Credentials.nickname;
            _AjaxData.data = new Dictionary<string, dynamic>();
            _AjaxData.data.Add("link", string.IsNullOrEmpty(data.Link) ? "" : data.Link);
            _AjaxData.data.Add("title", string.IsNullOrEmpty(data.Title) ? "" : data.Title);
            _AjaxData.data.Add("message", string.IsNullOrEmpty(data.Message) ? "" : data.Message);
            _AjaxData.data.Add("photo", string.IsNullOrEmpty(data.Photo) ? "" : data.Photo);
            _AjaxData.data.Add("video", string.IsNullOrEmpty(data.Video) ? "" : data.Video);
            _AjaxData.data.Add("audio", string.IsNullOrEmpty(data.Audio) ? "" : data.Audio);
            _AjaxData.data.Add("document", string.IsNullOrEmpty(data.Document) ? "" : data.Document);
            _AjaxData.data.Add("youtube", string.IsNullOrEmpty(data.Youtube) ? "" : data.Youtube);
            //_AjaxData.data.Add("list", "96176526120"); //List of contacts that will receive message separated by new line \n

            _AjaxData.data.Add("service_numbers", serviceNumbers);
            //The service number you want to use
            _AjaxData.data.Add("send_to_groups", data.SendToGroups);
            _AjaxData.data.Add("send_to_contacts", data.SendToContacts);
            _AjaxData.data.Add("use_template", data.UseTemplate);
            _AjaxData.data.Add("message_gmt_date_time", "");
            _AjaxData.data.Add("message_date_time", "");
            _AjaxData.data.Add("with_caption", data.SetCaption);


            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(_AjaxData);
            request.AddQueryParameter("data", "data");
            client.UserAgent = "massejli";

            var response = client.Execute<AjaxData>(request);
            var content = response.Content;
            return content;
        }

    }
}
