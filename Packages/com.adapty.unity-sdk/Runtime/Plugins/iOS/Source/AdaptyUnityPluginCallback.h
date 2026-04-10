#ifndef AdaptyUnityPluginCallback_h
#define AdaptyUnityPluginCallback_h

typedef const void* UnityAction;

typedef void (*CallbackDelegate)(UnityAction action, const char *data);

void SendMessageToUnity(NSString* type, NSString* data);

void SendCallbackToUnity(UnityAction action, NSString* data);

static NSString * cstringToString(const char *str);

static const char * cstringFromString(NSString *str);

#endif /* AdaptyUnityPluginCallback_h */
