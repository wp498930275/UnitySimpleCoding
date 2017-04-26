using System.Collections.Generic;
using UnityEngine;

public enum NotificationType {
    NT_TestStart,
    NT_TestClickBtn,
    NT_TestClickBtnA,
    NT_TestClickBtnB,

}

/// <summary>
/// 全局消息中心：
/// 通过Subscribe、Unsubscribe订阅、取消订阅
/// 通过Publish发布消息
/// 消息的数据存储在Notification中，方便以后的扩展，添加需要关注的信息
/// 
/// note:如果有需求可以扩展多个消息中心，把消息进一步细化
/// </summary>
public class NotificationCenter {

    public class NotificationData {
        public Object sender;
        public NotificationDelegate handler;

        public NotificationData(NotificationDelegate _handler) {
            handler = _handler;
            sender = null;
        }

        public NotificationData(NotificationDelegate _handler, Object _sender) {
            sender = _sender;
            handler = _handler;
        }

        public override string ToString() {
            return string.Format("sender:{0} handler:{1}", sender, handler);
        }
    }

    public delegate void NotificationDelegate(params object[] args);

    public static void Subscribe(NotificationType message, NotificationDelegate handler) {
        Subscribe(message, new NotificationData(handler));
    }

    public static void Subscribe(NotificationType message, NotificationDelegate handler, Object sender) {
        Subscribe(message, new NotificationData(handler, sender));
    }

    //订阅
    public static void Subscribe(NotificationType message, NotificationData data) {
            if (!listeners.ContainsKey(message)) {
                listeners.Add(message, new List<NotificationData>());
            }
            var actions = listeners[message];
            NotificationData searchResult = null;
            foreach (var item in actions) {
                if (item.handler == data.handler) {
                    searchResult = item;
                }
            }
            if (searchResult != null) {
                return;
            }
            actions.Add(data);
    }

    //取消订阅
    public static void Unsubscribe(NotificationType message, NotificationDelegate handler) {
            if (!listeners.ContainsKey(message)) {
                return;
            }
            var actions = listeners[message];
            foreach (var item in actions) {
                if (item.handler == handler) {
                    //理论上每个handler只会存在一个，找到了就可以结束循环
                    actions.Remove(item);
                    break;
                }
            }

            if (actions.Count == 0) {
                listeners.Remove(message);
            }
    }

    //发布
    public static void Publish(NotificationType message, params object[] args) {
            if (!listeners.ContainsKey(message)) {
                return;
            }
            var actions = listeners[message];
            if (actions != null) {
                foreach (var item in actions) {
                    item.handler(args);
                }
            }
    }

    //清空消息中心
    public static void Clear() {
        listeners.Clear();
    }

    private static Dictionary<NotificationType, List<NotificationData>> listeners = new Dictionary<NotificationType, List<NotificationData>>();
}
