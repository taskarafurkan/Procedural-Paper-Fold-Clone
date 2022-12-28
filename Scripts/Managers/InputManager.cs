using PaperFold.FoldMechanic;
using UnityEngine;

namespace PaperFold.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private AudioClip foldPaperClip;

        private void Update()
        {
#if UNITY_ANDROID
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                OnInput(Input.touches[0].position);
            }
#endif

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                OnInput(Input.mousePosition);
            }
#endif
        }

        private void OnInput(Vector3 inputPos)
        {
            Ray ray = _camera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.root.GetComponent<LevelController>().clickBlocked == false)
                {
                    if (hit.transform.GetComponentInParent<FoldPaper>().rotated == false)
                    {
                        hit.transform.GetComponentInParent<FoldPaper>().RotatePaper(false);
                        soundManager.PlaySound(foldPaperClip, Random.Range(1f, 1.3f));
                    }
                    else
                    {
                        if (hit.transform.GetComponentInParent<FoldPaper>().orderIndex < hit.transform.root.GetComponent<LevelController>().currentOrderIndex - 1)
                        {
                            StartCoroutine(hit.transform.root.GetComponent<LevelController>().RotateBackToIndex(hit.transform.GetComponentInParent<FoldPaper>().orderIndex));
                            soundManager.PlaySound(foldPaperClip, Random.Range(1f, 1.3f));
                        }
                        else
                        {
                            hit.transform.GetComponentInParent<FoldPaper>().RotatePaper(true);
                            soundManager.PlaySound(foldPaperClip, Random.Range(.7f, .8f));

                        }
                    }
                }
            }
        }
    }
}