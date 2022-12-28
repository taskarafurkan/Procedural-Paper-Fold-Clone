using DG.Tweening;
using PaperFold.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperFold.FoldMechanic
{
    public class LevelController : MonoBehaviour
    {
        public int currentOrderIndex = 0;
        public int maxNumberOfRotations;
        public float animationSpeed = 1.8f;

        [HideInInspector] public LevelCreator levelCreator;
        [HideInInspector] public SoundManager soundManager;
        [HideInInspector] public AudioClip stickerClip;
        [HideInInspector] public AudioClip winClip;
        [HideInInspector] public bool clickBlocked = false;
        [HideInInspector] public Material solidBlackMat;
        [HideInInspector] public List<FoldPaper> foldPapers = new List<FoldPaper>();
        [HideInInspector] public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        [HideInInspector] public AnimationCurve rotationCurve;
        [HideInInspector] public GameObject popupSticker;
        [HideInInspector] public ParticleSystem[] confettiParticles;

        private IEnumerator OnWin()
        {
            clickBlocked = true;

            foreach (ParticleSystem confettiParticle in confettiParticles)
            {
                confettiParticle.Play();
            }
            soundManager.PlaySound(winClip, 1);

            yield return new WaitForSeconds(.5f);

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                spriteRenderer.material = solidBlackMat;
            }

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.AddForce(new Vector3(0, 0, 5), ForceMode.Impulse);
            rb.AddTorque(new Vector3(0, -3, 0), ForceMode.Force);

            soundManager.PlaySound(stickerClip, 1);

            popupSticker.transform.parent = null;
            popupSticker.SetActive(true);
            // scale popup sprite with Dotween 
            popupSticker.transform.DOScale(new Vector3(.4f, .4f, 1), .3f)
                .SetEase(Ease.InSine)
                .SetLoops(3, LoopType.Yoyo);

            yield return new WaitForSeconds(2f);
            Destroy(popupSticker);
            levelCreator.CreateLevel(true, true);
        }

        private void OnInCorrect()
        {
            transform
                .DOMoveX(.3f, .1f)
                .OnComplete(() => transform.DOMoveX(-.3f, .1f)
                .OnComplete(() => transform.DOMoveX(.1f, .1f)
                .OnComplete(() => transform.DOMoveX(-.1f, .1f)
                .OnComplete(() => transform.DOMoveX(0f, .2f)
                .OnComplete(() => StartCoroutine(RotateBackToIndex(0)))))));
        }

        public IEnumerator RotateBackToIndex(int index)
        {
            int tmpIndex = currentOrderIndex;

            while (currentOrderIndex != index)
            {
                if (tmpIndex == currentOrderIndex)
                {
                    foldPapers[tmpIndex - 1].RotatePaper(true, true);
                    tmpIndex--;
                }
                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        public void CheckIfRotatedEverythingCorrectly()
        {
            bool everythingCorrect = true;

            foreach (FoldPaper fp in foldPapers)
            {
                if (fp.rightOrder == false)
                {
                    everythingCorrect = false;
                }
            }

            if (everythingCorrect)
            {
                StartCoroutine(OnWin());
            }
            else
            {
                OnInCorrect();
            }
        }

    }
}