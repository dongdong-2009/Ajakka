using System;

namespace Ajakka.Alerting.Descriptors
{
    public class PropertyHintAttribute:Attribute
    {
        public string Text {get;set;}
        public string Url {get;set;}

        public PropertyHintAttribute():this(string.Empty, string.Empty){}

        public PropertyHintAttribute(string text, string url){
            Text = text;
            Url = url;
        }


        public PropertyHintAttribute(string text){
            Text = text;
            Url = string.Empty;
        }
    }
    
}