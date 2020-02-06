using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    [DynamoDBTable("Profile")]
    public class Profile
    {
        private List<string> _likesMe;
        private List<string> _lLike;
        private List<string> _matches;


        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Nickname { get; set; }
        public int Age { get; set; }
        public string LiveIn { get; set; }
        [DynamoDBProperty("DateOfRegister", typeof(DateConverter))]
        public DateTime DateOfRegister { get; set; }
        public string AboutMe { get; set; }
        public string Gender { get; set; }
        public string SexualOrientation { get; set; }

        public string ProfileThumbnail { get; set; }

        public List<string> Match
        {
            get
            {
                if (_likesMe == null)
                    _likesMe = new List<string>();
                return _likesMe;
            }

            set
            {
                _likesMe = value;

            }
        }

        public List<string> LikesMe
        {
            get
            {
                if (_likesMe == null)
                    _likesMe = new List<string>();
                return _likesMe;
            }

            set
            {
                _likesMe = value;

            }
        }
        public List<string> ILike
        {
            get
            {
                if (_lLike == null)
                    _lLike = new List<string>();
                return _lLike;
            }

            set
            {
                _lLike = value;

            }
        }
        public List<string> LookingFor { get; set; }


    }

    public class DateConverter : IPropertyConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            var dateTime = entry?.AsString();
            if (string.IsNullOrEmpty(dateTime))
                return null;
            if (!DateTime.TryParse(dateTime, out DateTime value))
                throw new ArgumentException("entry parameter must be a validate DateTime value.", nameof(entry));
            else
                return value;
        }
        public DynamoDBEntry ToEntry(object value)
        {
            if (value == null)
                return new DynamoDBNull();
            if (value.GetType() != typeof(DateTime) && value.GetType() != typeof(DateTime?))
                throw new ArgumentException("value parameter must be a DateTime or a Nullable<DateTime>.", nameof(value));
            return ((DateTime)value).ToString();
        }
    }
}
