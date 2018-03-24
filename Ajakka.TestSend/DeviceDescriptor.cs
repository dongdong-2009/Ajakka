using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ajakka.TestSend{
    class DeviceDescriptor{
        const byte NameLength = 7;
        static readonly List<char> allowedCharacters = new List<char>();

        static Random random = new Random();
        public string Mac {get;set;}
        public string Ip {get;set;}
        public string Name {get;set;}

        public DeviceDescriptor(){

        }

        public DeviceDescriptor(string mac, string ip, string name){
            Mac = mac.ToUpper();
            Ip = ip;
            Name = name;
        }

        static DeviceDescriptor(){
            for(int i = 48; i < 57; i++){
                allowedCharacters.Add(Convert.ToChar(i));
            }
            
            for(int i = 65; i < 90; i++){
                allowedCharacters.Add(Convert.ToChar(i));
            }
        }
        internal static DeviceDescriptor CreateRandom()
        {
            return new DeviceDescriptor(){
                Mac = GetMac(),
                Ip = GetIp(),
                Name = GetName()
            };    
        }

        internal static DeviceDescriptor CreateRandom(string mac){
            return new DeviceDescriptor(){
                Mac = mac.ToUpper(),
                Ip = GetIp(),
                Name = GetName()
            };
        }

        internal static DeviceDescriptor CreateRandom(string mac, string ip){
            return new DeviceDescriptor(){
                Mac = mac.ToUpper(),
                Ip = ip,
                Name = GetName()
            };
        }

        internal static string GetName(){
            var name = new StringBuilder();
            for(int i = 0; i < NameLength; i++){
                name.Append(allowedCharacters[random.Next(allowedCharacters.Count)]);
            }
            return name.ToString();
        }

        internal static string GetIp(){
            var bytes = new byte[4];
            random.NextBytes(bytes);
            return new IPAddress(bytes).ToString();
        }

        internal static string GetMac(){
            var bytes = new byte[6];
            
            random.NextBytes(bytes);
            var mac = new StringBuilder();
            foreach(var b in bytes){
                mac.Append(b.ToString("x"));
            }
            return mac.ToString().ToUpper();
        }

        public override string ToString(){
            return Mac + "/" + Ip + "/" + Name;
        }
    }
}