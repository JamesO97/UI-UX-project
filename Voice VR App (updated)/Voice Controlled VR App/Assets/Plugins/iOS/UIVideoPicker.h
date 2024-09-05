/* Original developer: https://gist.github.com/naojitaniguchi/7d71267814ccd0ca719e */

#import <UIKit/UIKit.h>
#import <MobileCoreServices/MobileCoreServices.h>

@interface APLViewController : UIViewController <UINavigationControllerDelegate, UIImagePickerControllerDelegate> {
    
    UIImagePickerController *videoPicker;
    
@public
    const char *callbackGameObjectName;
    const char *callbackFunctionName;
}

@end
