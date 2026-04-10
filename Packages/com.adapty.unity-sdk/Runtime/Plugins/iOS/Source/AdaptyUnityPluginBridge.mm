#import <Foundation/Foundation.h>
#include "AdaptyUnityPluginCallback.h"
#include "UnityFramework/UnityFramework-Swift.h"

static NSString * cstringToString(const char *str) {
    return str ? [NSString stringWithUTF8String:str] : nil;
}

static const char * cstringFromString(NSString *str) {
    return str ? [str cStringUsingEncoding:NSUTF8StringEncoding] : nil;
}

static char * makeStringCopy(const char *string) {
    if (string == NULL) {
        return NULL;
    }

    char *res = (char *)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

typedef void (*MessageDelegate)(const char *type, const char *data);

static CallbackDelegate _callbackDelegate = NULL;
static MessageDelegate _messageDelegate = NULL;

void SendMessageToUnity(NSString *type, NSString *data) {
    dispatch_async(dispatch_get_main_queue(), ^{
        if (_messageDelegate != NULL) {
            _messageDelegate(cstringFromString(type), cstringFromString(data));
        }
    });
}

void SendCallbackToUnity(UnityAction action, NSString *data) {
    if (action == NULL) {
        return;
    }

    dispatch_async(dispatch_get_main_queue(), ^{
        if (_callbackDelegate != NULL) {
            _callbackDelegate(action, cstringFromString(data));
        }
    });
}

extern "C" {
void AdaptyUnity_registerCallbackHandler(MessageDelegate messageDelegate, CallbackDelegate callbackDelegate) {
    _messageDelegate = messageDelegate;
    _callbackDelegate = callbackDelegate;
}

void AdaptyUnity_invoke(const char *method, const char *request, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
        method:cstringToString(method)
        request:cstringToString(request)
        completion:^(NSString *_Nullable error) {
            SendCallbackToUnity(callback, error);
        }];
}
}
