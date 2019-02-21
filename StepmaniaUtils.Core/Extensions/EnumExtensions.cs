using System;
using System.Linq;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.Extensions
{
    internal static class EnumExtensions
    {
        public static SmFileAttribute AsSmFileAttribute(this string attr)
        {
            if (Enum.TryParse(attr, true, out SmFileAttribute result))
            {
                return result;
            }

            return SmFileAttribute.UNDEFINED;
        }
        
        public static PlayStyle AsPlayStyle(this string styleName)
        {
            styleName = styleName.Trim().TrimEnd(':');
            switch (styleName)
            {
                case "dance-single":
                    return PlayStyle.Single;
                case "dance-double":
                    return PlayStyle.Double;
                case "dance-couple":
                    return PlayStyle.Couple;
                case "dance-solo":
                    return PlayStyle.Solo;
                case "lights-cabinet":
                    return PlayStyle.Lights;
                default:
                    return PlayStyle.Undefined;
            }
        }

        public static SongDifficulty AsSongDifficulty(this string difficultyName)
        {
            return
                Enum.GetValues(typeof(SongDifficulty))
                    .OfType<SongDifficulty>()
                    .SingleOrDefault(d => difficultyName.Contains(d.ToString()));
        }
    }
}