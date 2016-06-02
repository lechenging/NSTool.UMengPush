using Newtonsoft.Json.Linq;
using NSTool.UMengPush;
using NSTool.UMengPush.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmCommunity.Utility
{
    public class PushMsg
    {
        public static string IosAppk = ConfigurationManager.AppSettings["UmengIosAppKey"].ToString();
        private static string IosAppsecret = ConfigurationManager.AppSettings["UmengIosAppSecret"].ToString();
        private static string AndAppk = ConfigurationManager.AppSettings["UmengAndAppKey"].ToString();
        private static string AndAppsecret = ConfigurationManager.AppSettings["UmengAndAppSecret"].ToString();
        private static string IsProduc = ConfigurationManager.AppSettings["IsProduction"].ToString();

        /// <summary>
        /// ios android push msg
        /// </summary>
        /// <param name="pushtype">消息类型</param>
        /// <param name="tag">用户标签 umeng设置</param>
        /// <param name="starttime">发送时间</param>
        /// /// <param name="title">通知标题</param>
        /// <param name="content">通知内容</param>
        /// <param name="description">通知文字描述</param>
        /// <param name="thirdparty_id">开发者自定义消息标识ID</param>
        public static void PushMessage(string pushtype, string tag, string starttime, string title, string content, string description, string thirdparty_id)
        {
            //"{""where"":{""and"":[{""or"":[{""tag"":""1""}]}]}}";
            JObject json = null;
            if (!string.IsNullOrWhiteSpace(tag))
            {
                string jsonStr =@"{""where"":{""and"":[{""or"":[{""tag"":"""+tag+@"""}]}]}}";
                json = JObject.Parse(jsonStr);
            }
            PushIosMsg(pushtype, IsProduc,json,starttime,title,content,description,thirdparty_id);
            PushAndroidMsg(pushtype, IsProduc, json, starttime, title, content, description, thirdparty_id);
            
        }

        /// <summary>
        /// ios push
        /// </summary>
        /// <param name="isproduc">推送模式 groupcast broadcast</param>
        public static void PushIosMsg(string pushtype, string isproduc,dynamic tagjsonobj,string starttime, string title, string content, string description, string thirdparty_id)
        {
            UMengMessagePush push = new UMengMessagePush(IosAppk, IosAppsecret);//可以配置到web.config中
            PostUMengJson postJson = new PostUMengJson();
            postJson.type = pushtype;
            postJson.production_mode = isproduc;
            postJson.payload.aps.alert = content;
            postJson.payload.aps.sound = "default";
            if (tagjsonobj!=null)
            {
                postJson.filter = tagjsonobj;//不能传字符串
            }
            if (!string.IsNullOrWhiteSpace(starttime))
            {
                postJson.policy.start_time = starttime;
            }

            postJson.payload.extra.Add("ActivityId", thirdparty_id);
            postJson.description = description;
            postJson.thirdparty_id = thirdparty_id;

            //ReturnJsonClass resu = push.SendMessage(postJson);
            push.AsynSendMessage(postJson, null);
        }

        public static void PushAndroidMsg(string pushtype, string isproduc, dynamic tagjsonobj, string starttime, string title, string content, string description, string thirdparty_id)
        {
            UMengMessagePush push = new UMengMessagePush(AndAppk, AndAppsecret);//可以配置到web.config中
            PostUMengJson postJson = new PostUMengJson();
            postJson.type = pushtype;
            postJson.production_mode = isproduc;

            if (tagjsonobj != null)
            {
                postJson.filter = tagjsonobj;//不能传字符串
            }
            if (!string.IsNullOrWhiteSpace(starttime))
            {
                postJson.policy.start_time = starttime;
            }

            //android 
            postJson.payload.extra.Add("ActivityId", thirdparty_id);
            postJson.payload.display_type = "notification";
            postJson.payload.body.ticker = "xxxx系统消息";
            postJson.payload.body.title = title; //"您的评论有回复了";
            postJson.payload.body.text = content;// "我是内容";
            postJson.payload.body.after_open = "go_custom";
            postJson.payload.body.custom = "comment-notify";
            postJson.payload.body.sound = "";//如果该字段为空，采用SDK默认的声音

            postJson.description = description;
            postJson.thirdparty_id = thirdparty_id;

            //ReturnJsonClass resu = push.SendMessage(postJson);
            //push.AsynSendMessage(postJson, callBack);
            push.AsynSendMessage(postJson, null);
        }

        private static void callBack(ReturnJsonClass result)
        {
            if (result.ret.Equals("FAIL"))
            {
                //这里可以记录log
                //logger.Error("推送失败,error_code: " + result.data.error_code);
            }
        }

    }
}
