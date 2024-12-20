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

- (void)applicationWillFinishLaunchingWithOptions:(NSNotification *)notification {
    [AdaptyUnityPlugin setup:^(NSString *_Nonnull type, NSString *_Nonnull data) {
        SendMessageToUnity(type, data);
    }];
}

@end
