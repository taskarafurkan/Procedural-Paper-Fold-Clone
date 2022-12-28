using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperFold.FoldMechanic
{
    public class FoldPaper : MonoBehaviour
    {
        public int orderIndex;
        public List<GameObject> corners = new List<GameObject>();
        public List<GameObject> backCorners = new List<GameObject>();

        public LevelController levelControllerRef;

        public Vector3 rotAxis;
        private float rotationDuration = 0.5f;
        public float angleChange;

        public bool rotated = false;

        public List<int> rotationOrderIndexes = new List<int>();
        public bool rightOrder = false;

        public void RotatePaper(bool back, bool rotateAll = false)
        {
            foreach (GameObject corner in corners)
            {
                corner.transform.parent = gameObject.transform;
            }

            if (back == false)
            {
                orderIndex = levelControllerRef.currentOrderIndex;
                levelControllerRef.foldPapers.Add(this);
                StartCoroutine(FoldAroundAxis(gameObject.transform, rotAxis, angleChange
                    , transform.position + new Vector3(0, orderIndex * 0.002f + 0.002f, 0), rotationDuration, back));
                rotated = true;

                foreach (int orderIndex in rotationOrderIndexes)
                {
                    if (orderIndex == levelControllerRef.currentOrderIndex)
                    {
                        rightOrder = true;
                    }
                }
            }
            else
            {
                orderIndex = -1;
                StartCoroutine(FoldAroundAxis(gameObject.transform, rotAxis, -angleChange
                    , new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), rotationDuration, back));

                levelControllerRef.foldPapers.Remove(this);
                rotated = false;
                rightOrder = false;
            }
        }

        IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float changeInAngle, Vector3 endPosition, float duration, bool back)
        {
            if (backCorners.Count > 0)
            {
                if (!back)
                {
                    if (backCorners[1] != null)
                    {
                        if (!backCorners[0].activeInHierarchy)
                        {
                            backCorners[1].SetActive(true);
                        }
                    }

                    if (backCorners.Count > 2)
                    {
                        if (backCorners[3] != null)
                        {
                            if (!backCorners[2].activeInHierarchy)
                            {
                                backCorners[3].SetActive(true);
                            }
                        }
                    }
                }
            }

            levelControllerRef.clickBlocked = true;

            Quaternion startRotation = paperTransform.rotation;
            Vector3 startPosition = paperTransform.position;

            float t = 0;

            while (t < duration)
            {
                paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle * levelControllerRef.rotationCurve.Evaluate(t / duration), axis);
                paperTransform.position = Vector3.Lerp(startPosition, endPosition, t / duration);

                t += Time.deltaTime * levelControllerRef.animationSpeed;


                yield return null;
            }

            paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle, axis);
            paperTransform.position = endPosition;

            if (back == true)
            {
                levelControllerRef.currentOrderIndex--;
            }
            else
            {
                levelControllerRef.currentOrderIndex++;

                if (levelControllerRef.maxNumberOfRotations == levelControllerRef.currentOrderIndex)
                {
                    levelControllerRef.CheckIfRotatedEverythingCorrectly();
                }
            }

            if (backCorners.Count > 0)
            {
                if (back)
                {
                    if (backCorners[1] != null)
                    {
                        backCorners[1].SetActive(false);
                    }

                    if (backCorners.Count > 2)
                    {
                        if (backCorners[3] != null)
                        {
                            backCorners[3].SetActive(false);
                        }
                    }
                }
            }

            if (levelControllerRef.currentOrderIndex < levelControllerRef.maxNumberOfRotations)
            {
                levelControllerRef.clickBlocked = false;
            }
        }
    }
}