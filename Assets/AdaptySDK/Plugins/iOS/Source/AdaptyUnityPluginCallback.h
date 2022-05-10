#ifndef AdaptyUnityPluginCallback_h
#define AdaptyUnityPluginCallback_h

typedef const void* UnityAction;

void SendMessageToUnity(NSString* type, NSString* data);

void SendCallbackToUnity(UnityAction action, NSString* data);

#endif /* AdaptyUnityPluginCallback_h */
