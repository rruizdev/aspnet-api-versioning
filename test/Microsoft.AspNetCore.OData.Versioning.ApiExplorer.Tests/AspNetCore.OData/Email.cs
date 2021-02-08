namespace Microsoft.AspNetCore.OData
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Email
    {
        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public string Username { get; set; }
    }
}