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

typedef void (*CallbackDelegate)(UnityAction action, const char *data);

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

    #pragma mark - Adapty

void AdaptyUnity_setIdfaCollectionDisabled(bool disabled) {
    [[AdaptyUnityPlugin shared]
     setIdfaCollectionDisabled:disabled
    ];
}

void AdaptyUnity_setLogLevel(const char *value) {
    [[AdaptyUnityPlugin shared]
     setLogLevel:cstringToString(value)
    ];
}

void AdaptyUnity_identify(const char *customerUserId, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
          identify:cstringToString(customerUserId)
        completion:^(NSString *_Nullable error) {
            SendCallbackToUnity(callback, error);
        }];
}

void AdaptyUnity_logout(UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     logout:^(NSString *_Nullable error) {
            SendCallbackToUnity(callback, error);
        }];
}

void AdaptyUnity_getPaywall(const char *paywallId, const char *locale, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     getPaywall:cstringToString(paywallId)
     locale:cstringToString(locale)
     completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_getPaywallProducts(const char *paywall, const char *fetchPolicy, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     getPaywallProducts:cstringToString(paywall)
            fetchPolicy:cstringToString(fetchPolicy)
             completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_getProfile(UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     getProfileWithCompletion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_restorePurchases(UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     restorePurchases:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_makePurchase(const char *product, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     makePurchase:cstringToString(product)
       completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_logShowPaywall(const char *paywall, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     logShowPaywall:cstringToString(paywall)
         completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_logShowOnboarding(const char *parameters, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     logShowOnboarding:cstringToString(parameters)
            completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_setFallbackPaywalls(const char *paywalls, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     setFallbackPaywalls:cstringToString(paywalls)
              completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_updateProfile(const char *params, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     updateProfile:cstringToString(params)
        completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_updateAttribution(const char *attributions, const char *source, const char *networkUserId, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     updateAttribution:cstringToString(attributions)
                source:cstringToString(source)
         networkUserId:cstringToString(networkUserId)
            completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_setVariationForTransaction(const char *parameters, UnityAction callback) {
    [[AdaptyUnityPlugin shared]
     setVariationForTransaction:cstringToString(parameters)
              completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUnity_presentCodeRedemptionSheet() {
    [[AdaptyUnityPlugin shared]
     presentCodeRedemptionSheet];
}
}
