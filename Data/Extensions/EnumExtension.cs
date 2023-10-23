using FeedHiveAuth.Models.Enums;

namespace FeedHiveAuth.Data.Extensions
{
    public static class EnumExtension
    {
        public static List<T> GetAllList<T>() where T : struct
        {
            return (from T enumValue in Enum.GetValues(typeof(T))
                    where Convert.ToInt32(enumValue) != -1
                    select enumValue)
                      .ToList();
        }

        public static Array GetAllList(string typeStr)
        {
            var type = Type.GetType(typeStr);
            if (type == null)
            {
                return null;
            }

            return Enum.GetValues(type);
        }
        public static string Key<T>(this T enumValue)
        {
            return enumValue.ToString().ToLower();
        }

        public static int Value<T>(this T enumValue)
        {
            var genericType = enumValue.GetType();
            if (genericType.IsEnum)
            {
                var eenum = Enum.Parse(enumValue.GetType(), enumValue.ToString()) as Enum;
                return Convert.ToInt32(eenum);
            }
            return -1;
        }

        public static T FromKey<T>(string key)
        {
            return (T)Enum.Parse(typeof(T), key.Trim(), true);
        }

        public static T FromValue<T>(int value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }

        public static string Icon(this StatusEnum status)
        {
            if (status == StatusEnum.New)
            {
                return "bolt";
            }
            
            if (status == StatusEnum.Processing)
            {
                return "spinner fa-spin";
            }
            if (status == StatusEnum.Failed || status == StatusEnum.ProcessedFailed)
            {
                return "exclamation-triangle";
            }
            return "";
        }

        public static string Color(this StatusEnum status)
        {
            if (status == StatusEnum.New)
            {
                return "blue";
            }
            
            if (status == StatusEnum.Processing)
            {
                return "purple";
            }
            if (status == StatusEnum.Failed || status == StatusEnum.ProcessedFailed)
            {
                return "orange";
            }
            return "";
        }

        public static string LabelColor(this StatusEnum status)
        {
            if (status == StatusEnum.New)
            {
                return "label-info";
            }
           
            if (status == StatusEnum.Processing)
            {
                return "label-purple";
            }
            if (status == StatusEnum.Failed || status == StatusEnum.ProcessedFailed || status == StatusEnum.Expired)
            {
                return "label-warning";
            }
            return "";
        }



        public static StatusEnum GetStatus(this ActionEnum action)
        {
            if (action == ActionEnum.Delete)
            {
                return StatusEnum.Deleted;
            }

            if (action == ActionEnum.Publish)
            {
                return StatusEnum.Published;
            }

            if (action == ActionEnum.UnPublish)
            {
                return StatusEnum.Unpublished;
            }

            return StatusEnum.New;
        }
    }
}
