#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import "AppDelegateListener.h"
@import Adapty;


@interface AdaptyAppController : UnityAppController <AppDelegateListener>
@end

@implementation AdaptyAppController

- (void)didReceiveRemoteNotification:(NSNotification*)notification {
    [Adapty handlePushNotification:notification.userInfo completion:^(NSError * _Nullable) {
        
    }];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(AdaptyAppController)
