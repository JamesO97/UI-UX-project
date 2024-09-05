/* Original developer: https://gist.github.com/naojitaniguchi/7d71267814ccd0ca719e */

#import "UIVideoPicker.h"

char videoUrlPath[1024];

@implementation APLViewController

// Load the Photo Library view.
- (void)viewDidLoad
{
    [super viewDidLoad];

    [self showImagePickerForSourceType:UIImagePickerControllerSourceTypePhotoLibrary];
}


#pragma mark - UIImagePickerControllerSourceType

// Display photo library to user
- (void)showImagePickerForSourceType:(UIImagePickerControllerSourceType)sourceType
{
    videoPicker = [[UIImagePickerController alloc] init];
    videoPicker.modalPresentationStyle = UIModalPresentationCurrentContext;
    videoPicker.sourceType = sourceType;
    
    // Show user videos to select with or without audio.
    videoPicker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie, (NSString *)kUTTypeVideo, nil];
    
    // Be sure that each video compresses with the highest quality.
    videoPicker.videoQuality = UIImagePickerControllerQualityTypeHigh;
    
    // Set the delegate so that the right method is called
    videoPicker.delegate = self;
    // Add the imagePicker to the view
    [self.view addSubview:videoPicker.view];
}


#pragma mark - UIImagePickerControllerDelegate

// Called when a video has been selected from the photo library.
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    // Store media type selected by the user
    NSString *type = [info objectForKey:UIImagePickerControllerMediaType];
    
    // Get the path of the media selected and copy it into the char array.
    // Movies and videos are not the same, so they must be separated.
    if ([type isEqualToString:(NSString *)kUTTypeVideo] ||
        [type isEqualToString:(NSString *)kUTTypeMovie])
    {
        NSURL *videoURL = [info objectForKey:UIImagePickerControllerMediaURL];
        NSLog(@"%@", videoURL);
        NSString *urlString = [videoURL absoluteString];
        const char* cp = [urlString UTF8String];
        strcpy(videoUrlPath, cp);
    }

    [self dismissViewControllerAnimated:YES completion:nil];
    
    // Send a message to unity. This is necessary for the Plug-In to communicate with Unity.
    UnitySendMessage(callbackGameObjectName, callbackFunctionName, videoUrlPath);
}


// Dismiss the View should the user cancel.
- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    [self dismissViewControllerAnimated:YES completion:nil];
}


@end


#pragma mark - CWrapperToCommunicateWithUnity

// This section is wrapped in a C wrapper to communicate with Unity.
// As this is an Objective-C++ file, this is required.
extern "C" {
    void OpenVideoPicker(const char *gameObjectName, const char *functionName) {
        // Execute the code when the user is on a physical device.
        if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypePhotoLibrary]) {
            UIViewController *rootViewController = UnityGetGLViewController();
            APLViewController *aplViewController = [[APLViewController alloc] init];
            aplViewController->callbackGameObjectName = strdup(gameObjectName);
            aplViewController->callbackFunctionName = strdup(functionName);
            
            [rootViewController presentViewController:aplViewController animated:YES completion:nil];
        }
    }
}
