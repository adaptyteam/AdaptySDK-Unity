#import <Foundation/Foundation.h>
#include "AdaptyUnityPluginCallback.h"
#import "AppDelegateListener.h"
#include "UnityFramework/UnityFramework-Swift.h"

@interface ApplicationStateListener : NSObject <AppDelegateListener>
+ (instancetype)sharedInstance;
@end

@implementation ApplicationStateListener

static ApplicationStateListener *_applicationStateListenerInstance = [[ApplicationStateListener alloc] init];

+ (instancetype)sharedInstance {
    return _applicationStateListenerInstance;
}

- (instancetype)init {
    self = [super init];

    if (self) {
        UnityRegisterAppDelegateListener(self);
    }

    return self;
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (NSDictionary *)infoDictionary {
    NSString *plistPath = [[NSBundle mainBundle] pathForResource:@"Adapty-Info" ofType:@"plist"];
    NSData *plistData = [NSData dataWithContentsOfFile:plistPath options:0 error:NULL];

    if (!plistData) {
        return [[NSBundle mainBundle] infoDictionary];
    }

    NSError *readingError = nil;
    NSPropertyListSerialization *plist = [NSPropertyListSerialization propertyListWithData:plistData options:NSPropertyListMutableContainers format:nil error:&readingError];

    if (plist) {
        return (NSDictionary *)plist;
    } else {
        return [[NSBundle mainBundle] infoDictionary];
    }
}

- (void)applicationWillFinishLaunchingWithOptions:(NSNotification *)notification {
    NSDictionary *infoDictionary = [self infoDictionary];
    NSString *apiKey = infoDictionary[@"AdaptyPublicSdkKey"];
    BOOL observerMode = [infoDictionary[@"AdaptyObserverMode"] boolValue];
    BOOL idfaCollectionDisabled = [infoDictionary[@"AdaptyIDFACollectionDisabled"] boolValue];
    NSString *storeKit2UsageString = infoDictionary[@"AdaptyStoreKit2Usage"];

    NSString *overrideBaseUrlString = infoDictionary[@"AdaptyOverrideBaseURL"];
    NSString *overrideFallbackBaseUrlString = infoDictionary[@"AdaptyOverrideFallbackBaseURL"];

    if (overrideBaseUrlString) {
        NSURL *backendEnvironmentURL = [NSURL URLWithString:overrideBaseUrlString];

        if (backendEnvironmentURL) {
            [[AdaptyUnityPlugin shared]
             setBackendEnvironmentBaseUrl:backendEnvironmentURL];
        }
    }

    if (overrideFallbackBaseUrlString) {
        NSURL *fallbackEnvironmentURL = [NSURL URLWithString:overrideFallbackBaseUrlString];

        if (fallbackEnvironmentURL) {
            [[AdaptyUnityPlugin shared]
             setBackendEnvironmentFallbackBaseUrl:fallbackEnvironmentURL];
        }
    }

    [[AdaptyUnityPlugin shared]
     setCrossPlatformSDK:@"unity"
                 version:@"2.9.0"
    ];

    [[AdaptyUnityPlugin shared]
     setIdfaCollectionDisabled:idfaCollectionDisabled
    ];

    [[AdaptyUnityPlugin shared]
                    activate:apiKey
                observerMode:observerMode
        storeKit2UsageString:storeKit2UsageString
    ];

    [[AdaptyUnityPlugin shared]
     registerMessageDelegate:^(NSString *_Nonnull type, NSString *_Nonnull data) {
        SendMessageToUnity(type, data);
    }];
}

@end
