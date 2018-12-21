using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class OoxmlColorFactory
    {
        //TODO Refactor the code such that each class parse itself
        public static OoxmlColor getOoxmlColor(XmlElement element)
        {
            OoxmlColor color = null;
            String nodeName = element.Name;
            switch (nodeName)
            {
                case "a:scrgbClr":
                    color = new ScrgbClr(
                            int.Parse(element.GetAttribute("r")),
                            int.Parse(element.GetAttribute("g")),
                            int.Parse(element.GetAttribute("b")));
                    break;
                case "a:srgbClr":
                    color = new SrgbClr(element.GetAttribute("val"));
                    break;
                case "a:hslClr":
                    color = new HslClr(
                            int.Parse(element.GetAttribute("hue")),
                            int.Parse(element.GetAttribute("sat")),
                            int.Parse(element.GetAttribute("lum")));
                    break;
                case "a:sysClr":
                    color = new SysClr(
                            element.GetAttribute("val"),
                            element.GetAttribute("lastClr")
                            );
                    break;
                case "a:schemeClr":
                    color = new SchemeClr(element.GetAttribute("val"));
                    break;
                case "a:prstClr":
                    color = new SrgbClr(element.GetAttribute("val"));
                    break;

            }

            List<XmlElement> effects = mxVsdxUtils.getDirectChildElements(element);

            foreach (XmlElement effect in effects)
            {
                int effVal = int.Parse(effect.GetAttribute("val")) / 1000; //these values are multiplied by 10,000 so we divide by 1,000 to keep the percentage only
                String effName = effect.Name;
                switch (effName)
                {
                    case "a:tint":
                        color.setTint(effVal);
                        break;
                    case "a:shade":
                        color.setShade(effVal);
                        break;
                    case "a:satMod":
                        color.setSatMod(effVal);
                        break;
                    case "a:lumMod":
                        color.setLumMod(effVal);
                        break;
                    case "a:hueMod":
                        color.setHueMod(effVal);
                        break;
                        //TODO complete the list when supported
                        //				a:comp    Complement
                        //				a:inv    Inverse
                        //				a:gray    Gray
                        //				a:alpha    Alpha
                        //				a:alphaOff    Alpha Offset
                        //				a:alphaMod    Alpha Modulation
                        //				a:hue    Hue
                        //				a:hueOff    Hue Offset
                        //				a:sat    Saturation
                        //				a:satOff    Saturation Offset
                        //				a:lum    Luminance
                        //				a:lumOff    Luminance Offset
                        //				a:red    Red
                        //				a:redOff    Red Offset
                        //				a:redMod    Red Modulation
                        //				a:green    Green
                        //				a:greenOff    Green Offset
                        //				a:greenMod    Green Modification
                        //				a:blue    Blue
                        //				a:blueOff    Blue Offset
                        //				a:blueMod    Blue Modification
                        //				a:gamma    Gamma
                        //				a:invGamma    Inverse Gamma
                }
            }
            return color;

        }
    }
}
