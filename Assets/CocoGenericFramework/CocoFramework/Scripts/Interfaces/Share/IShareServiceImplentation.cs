using UnityEngine;
using System.Collections;

namespace TabTale
{
    public interface IShareServiceImplentation
    {
        void Init();

        /// <summary>
        /// Pops the Share Context Menu with a given image or screenshot
        /// </summary>
        /// <param name="shareSubject">Subject of the share if available (e.g. email share) </param>
        /// <param name="shareBodyText"> The main text you see in share </param>
        /// <param name="appStoreUrl"> ITC or GP link (On Android Concatenated to the body text) </param>
        /// <param name="shareHeadLine">Android only: Sharing menu headline</param>
        /// <param name="customImagePath">Optional : If left null the app will take screenshot at the end of frame</param>
		void ShareWithImage(string shareSubject, string shareBodyText, string appStoreUrl, string shareHeadLine, byte[] screenShotData = null);
    }
}

