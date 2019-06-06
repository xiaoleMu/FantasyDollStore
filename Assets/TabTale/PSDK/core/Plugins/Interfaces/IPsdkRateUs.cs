using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {
	
	public interface IPsdkRateUs : IPsdkService {

		bool SmallSatisfactionPointReached ();

		bool LargeSatisfactionPointReached ();

		bool ShouldShowRateUs ();

        // userAction:
        // Relevant to iOS only. Use userAction = true for buttons.
        // Since iOS10, it's recommended to use SKStoreReviewController requestReview,
        // which should not be used when clicking a button(user action),
		// since it has an internal mechanism that can decide to not show.
		// So whenever rate us is raised from a button we have to indicate so that the regular rate dialog is not requested,
		// but instead use another method that always works.
		void Show (bool userAction = false); //equivalent to DialogResultEvent with Rate (other states were never used?)

		void NeverShow (); //equivalent to DialogResultEvent with neverShow

	}
}