using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CocoPlay;

public class CCAction
{
    public static void Stop(GameObject obj)
    {
        LeanTween.cancel(obj);
    }

    public static void Move(GameObject obj, Vector3[] paths, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.move(obj, paths, duration).setEase(easetype);
    }

    public static void Move(GameObject obj, Vector3 endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.move(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveX(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveX(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveY(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveY(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveZ(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveZ(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveLocal(GameObject obj, Vector3 endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveLocal(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveLocalX(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveLocalX(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveLocalY(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveLocalY(obj, endPos, duration).setEase(easetype);
    }

    public static void MoveLocalZ(GameObject obj, float endPos, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.moveLocalZ(obj, endPos, duration).setEase(easetype);
    }

    public static LTDescr ScaleTo(GameObject obj, Vector3 endScale, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        return LeanTween.scale(obj, endScale, duration).setEase(easetype);
    }
    public static void ScalePingPongTo(GameObject obj, Vector3 endScale, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.scale(obj, endScale, duration).setEase(easetype).setLoopPingPong();
    }

    public static void RotateTo(GameObject obj, Vector3 endAngle, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.rotate(obj, endAngle, duration).setEase(easetype);
    }

    public static void RotateToX(GameObject obj, float x, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.rotateX(obj, x, duration).setEase(easetype);
    }

    public static void RotateToY(GameObject obj, float y, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.rotateY(obj, y, duration).setEase(easetype);
    }

    public static void RotateToZ(GameObject obj, float z, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.rotateZ(obj, z, duration).setEase(easetype);
    }

    public static void Color(GameObject obj, Color color, float duration, LeanTweenType easetype = LeanTweenType.linear)
    {
        LeanTween.color(obj, color, duration).setEase(easetype);
    }

    /// <summary>
    /// 设置图片透明度.
    /// </summary>
    /// <returns>返回协同</returns>
    /// <param name="pImg">需要设置的图片</param>
    /// <param name="pAlpha">需要设置的透明度min:0max:1</param>
    /// <param name="pTime">执行时间</param>

    public static IEnumerator MoveImageAlpha(Image pImg, float pAlpha, float pTime)
    {
        if (pImg == null || pTime < 0)
            yield break;

        if (pAlpha < 0)
            pAlpha = 0;

        if (pAlpha > 1)
            pAlpha = 1;

        float tClip = 0;
        Color tColor = pImg.color;
        float tAlpha = tColor.a;

        while (tClip < 1)
        {
            tClip += Time.unscaledDeltaTime / pTime;
            if (tClip > 1f)
                tClip = 1f;

            tColor.a = Mathf.Lerp(tAlpha, pAlpha, tClip);
            pImg.color = tColor;
            yield return new WaitForEndOfFrame();
        }
    }


    /// <summary>
    /// 设置摄像机Rect的X值
    /// </summary>
    /// <returns>The view prot x.</returns>
    /// <param name="pCamera">P camera.</param>
    /// <param name="pValue">P value.</param>
    /// <param name="pTime">P time.</param>
    public static IEnumerator MoveCameraRectX(Camera pCamera, float pValue, float pTime)
    {
        if (pCamera == null) yield break;

        float tClip = 0;
        Rect tRect = pCamera.rect;
        float tOriginX = tRect.x;

        if (pTime <= 0f)
        {
            tRect.x = pValue;
            pCamera.rect = tRect;
            yield break;
        }

        float pStartTime = Time.time;
        while (Time.time - pStartTime < pTime)
        {
            tClip += (Time.time - pStartTime) / pTime;
            if (tClip > 1f)
                tClip = 1f;
            tRect.x = Mathf.Lerp(tOriginX, pValue, tClip);
            pCamera.rect = tRect;
            yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// 通过像素设置摄像机Rect的X值
    /// </summary>
    /// <returns>The camera rect XB y pixels.</returns>
    /// <param name="pCamera">P camera.</param>
    /// <param name="pPixels">P value.</param>
    /// <param name="pTime">P time.</param>
    public static void MoveCameraRectXByPixels(Camera pCamera, float pPixels, float pTime, bool pMoveLeft = false)
    {
        if (pCamera == null) return;
        float tScreenRealWidth = 768f * Screen.width / Screen.height;
        float tOffset = pPixels / tScreenRealWidth;
        if (pMoveLeft)
            tOffset *= -1f;
        CocoMainController.Instance.StartCoroutine(CCAction.MoveCameraRectX(pCamera, tOffset, pTime));
    }


	static public IEnumerator MoveGameobject(GameObject obj, Vector3 pos, Vector3 eulerAngles, float time, LeanTweenType easeType = LeanTweenType.linear, System.Action onComplete = null)
	{
		LeanTween.move(obj, pos, time).setEase(easeType);
		LeanTween.rotate(obj, eulerAngles, time).setEase(easeType);
		yield return new WaitForSeconds(time);
		if(onComplete != null)
			onComplete();
	}

	static public IEnumerator MoveGameobject(GameObject obj, Transform followTrans, float time, LeanTweenType easeType = LeanTweenType.linear, System.Action onComplete = null)
	{
		return CCAction.MoveGameobject(obj, followTrans.position, followTrans.eulerAngles, time, easeType, onComplete);
	}

	static public IEnumerator MoveGameobject(GameObject obj, Vector3 pos, float time)
	{
		CCAction.MoveLocal(obj, pos, time);
		yield return new WaitForSeconds(time);
	}
}
