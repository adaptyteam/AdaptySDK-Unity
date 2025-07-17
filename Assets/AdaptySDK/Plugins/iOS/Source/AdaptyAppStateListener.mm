#import <Foundation/Foundation.h>
#include "AdaptyUnityPluginCallback.h"
#import "AppDelegateListener.h"
#include "UnityFramework/UnityFramework-Swift.h"

@interface AdaptyApplicationStateListener : NSObject <AppDelegateListener>
+ (instancetype)sharedInstance;
@end

@implementation AdaptyApplicationStateListener

static AdaptyApplicationStateListener *_applicationStateListenerInstance = [[AdaptyApplicationStateListener alloc] init];

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
