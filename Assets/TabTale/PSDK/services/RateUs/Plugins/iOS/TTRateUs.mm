#import <StoreKit/StoreKit.h>
#import <PublishingSDKCore/PublishingSDKCore.h>

extern "C"
{
    void _DisplayRateUsModal(const char *applicationId, BOOL userAction);
    
}

// userAction:
// Relevant to iOS only. Should be: userAction = true for buttons.
// Since iOS10, it's recommended to use SKStoreReviewController requestReview,
// which should not be used when clicking a button (user action),
// since it has an internal mechanism that can decide to not show.
// So whenever rate us is raised from a button we have to indicate so that the regular rate dialog is not requested,
// but instead use another method that always works.
void _DisplayRateUsModal(const char *applicationId, BOOL userAction)
{
    [[PSDKServiceManager instance] requestReview: userAction];
}
