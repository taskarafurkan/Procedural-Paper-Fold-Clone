using PaperFold.FoldMechanic;
using PaperFold.ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace PaperFold.Managers
{
    public class LevelCreator : MonoBehaviour
    {
        #region Init

        [Header("[Orders]")]
        [SerializeField] private string leftOrder;
        [SerializeField] private string rightOrder;
        [SerializeField] private string topOrder;
        [SerializeField] private string bottomOrder;
        [Space(3f)]

        [Header("[Base Vertices]")]
        [SerializeField] private float baseLeftVertices;
        [SerializeField] private float baseRightVertices;
        [SerializeField] private float baseTopVertices;
        [SerializeField] private float baseBottomVertices;
        [Space(3f)]

        [Header("[Part Vertices]")]
        [Range(0f, -1f)]
        [SerializeField] private float partLeftVertices;
        [Range(0f, 1f)]
        [SerializeField] private float partRightVertices;
        [Range(0f, 1f)]
        [SerializeField] private float partTopVertices;
        [Range(0f, -1f)]
        [SerializeField] private float partBottomVertices;

        [Header("[General]")]
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private ParticleSystem[] confettiParticles;


        [Header("[Base]")]
        [SerializeField] private GameObject dashedLine;
        [SerializeField] private GameObject StickerPrefab;
        [SerializeField] private Sprite maskSprite;
        [SerializeField] private Sprite[] stickers;

        [Header("[Audio]")]
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private AudioClip stickerClip;
        [SerializeField] private AudioClip winClip;

        [Header("[Background]")]
        [SerializeField] private Texture[] bgTextures;

        [Header("[Materials]")]
        [Space(3f)]
        [SerializeField] private Material[] topMat;
        [SerializeField] private Material[] bottomMat;
        [SerializeField] private Material bgMat;
        [SerializeField] private Material solidWhiteMat;
        [SerializeField] private Material solidBlackMat;
        [SerializeField] private Material outlineMat;

        private GameObject leftPart;
        private GameObject rightPart;
        private GameObject topPart;
        private GameObject bottomPart;

        private int randSticker;
        private int randBgTexture;
        private int randTopMat;
        private int randBottomMat;

        [HideInInspector] public List<GameObject> levels = new List<GameObject>();
        private ProceduralPart proceduralPart = new ProceduralPart();

        #endregion

        private void Start()
        {
            if (levels.Count == 0)
            {
                CreateLevel(true, false);
            }
        }

        public void CreateLevel(bool isRandom, bool deleteLevels)
        {
            if (deleteLevels)
            {
                DeleteAllLevels();
            }

            if (isRandom)
                RandomizeOrders();

            randTopMat = Random.Range(0, topMat.Length);
            randBottomMat = Random.Range(0, bottomMat.Length);
            randSticker = Random.Range(0, stickers.Length);
            randBgTexture = Random.Range(0, bgTextures.Length);

            bgMat.SetTexture("_MainTex", bgTextures[randBgTexture]);

            EditOrdersAndVertices(isRandom);

            GameObject level = new GameObject(leftOrder + "," + rightOrder + "," + topOrder + "," + bottomOrder);
            this.levels.Add(level);

            CreatePart(level, "Base", Parts.BASE);
            CreatePart(level, "Left", Parts.LEFT);
            CreatePart(level, "Right", Parts.RIGHT);
            CreatePart(level, "Top", Parts.TOP);
            CreatePart(level, "Bottom", Parts.BOTTOM);

            CreatePart(level, "Top Left", Parts.TOP_LEFT);
            CreatePart(level, "Top Right", Parts.TOP_RIGHT);
            CreatePart(level, "Bottom Left", Parts.BOTTOM_LEFT);
            CreatePart(level, "Bottom Right", Parts.BOTTOM_RIGHT);
        }

        public void CreateAllLevels()
        {
            Vector4[] possibleOrders =
                {
                    new Vector4(0, 2, 3, 1), new Vector4(0, 2, 1, 3), new Vector4(2, 0, 3, 1)
                  , new Vector4(2, 0, 1, 3), new Vector4(0, 1, 3, 2), new Vector4(0, 1, 2, 3)
                  , new Vector4(1, 0, 3, 2), new Vector4(1, 0, 2, 3), new Vector4(0, 3, 2, 1)
                  , new Vector4(0, 3, 1, 2), new Vector4(3, 0, 2, 1), new Vector4(3, 0, 1, 2)

                  , new Vector4(1, 2, 0, 3), new Vector4(1, 2, 3, 0), new Vector4(1, 3, 0, 2)
                  , new Vector4(1, 3, 2, 0), new Vector4(2, 1, 0, 3), new Vector4(2, 1, 3, 0)
                  , new Vector4(2, 3, 1, 0), new Vector4(2, 3, 0, 1), new Vector4(3, 1, 2, 0)
                  , new Vector4(3, 1, 0, 2), new Vector4(3, 2, 1, 0), new Vector4(3, 2, 0, 1)
            };


            for (int i = 0; i < possibleOrders.Length; i++)
            {
                leftOrder = possibleOrders[i].x.ToString();
                rightOrder = possibleOrders[i].y.ToString();
                topOrder = possibleOrders[i].z.ToString();
                bottomOrder = possibleOrders[i].w.ToString();

                CreateLevel(false, false);
            }
        }

        private void RandomizeOrders()
        {
            int[] ordersArray = { 0, 1, 2, 3 };
            List<int> ordersList = ordersArray.ToList();

            leftOrder = ordersList[Random.Range(0, ordersList.Count)].ToString();
            ordersList.Remove(int.Parse(leftOrder));

            rightOrder = ordersList[Random.Range(0, ordersList.Count)].ToString();
            ordersList.Remove(int.Parse(rightOrder));

            topOrder = ordersList[Random.Range(0, ordersList.Count)].ToString();
            ordersList.Remove(int.Parse(topOrder));

            bottomOrder = ordersList[Random.Range(0, ordersList.Count)].ToString();
            ordersList.Remove(int.Parse(bottomOrder));
        }

        private void EditOrdersAndVertices(bool isRandom)
        {
            float baseHorizontalLenght = Mathf.Abs(baseLeftVertices) + baseRightVertices;
            float baseVerticalLenght = baseTopVertices + Mathf.Abs(baseBottomVertices);

            float totalHorizontalLenght = Mathf.Abs(partLeftVertices) + partRightVertices + baseHorizontalLenght;
            float totalVerticalLenght = Mathf.Abs(partBottomVertices) + partTopVertices + baseVerticalLenght;

            if (isRandom)
            {
                // Randomize parts vertices for variety
                partTopVertices = Random.Range(baseVerticalLenght / 3.3f, baseVerticalLenght / 1.4f);
                partBottomVertices = -Mathf.Abs(baseVerticalLenght - partTopVertices);
                partRightVertices = Random.Range(.3f, .4f);
                partLeftVertices = -Mathf.Abs(partRightVertices);
            }


            // Calculate material tiling for prevent texture stretching
            topMat[randTopMat].SetTextureScale("_MainTex", new Vector2(totalHorizontalLenght / 2, 1f));
            bottomMat[randBottomMat].SetTextureScale("_MainTex", new Vector2(totalHorizontalLenght / 2 * 1.3f, 1.3f));

            // Get left order first char and convert to int, get right order last char and convert to int to find out if they have same order
            if (int.Parse(leftOrder.Substring(0, 1)) == 1 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 2
                || int.Parse(leftOrder.Substring(0, 1)) == 2 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 1)
            {
                leftOrder = "1,2";
                rightOrder = "1,2";
                return;
            }
            if (int.Parse(leftOrder.Substring(0, 1)) == 1 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 3
                || int.Parse(leftOrder.Substring(0, 1)) == 3 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 1)
            {
                return;
            }
            if (int.Parse(leftOrder.Substring(0, 1)) == 2 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 3
                || int.Parse(leftOrder.Substring(0, 1)) == 3 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 2)
            {
                leftOrder = "2,3";
                rightOrder = "2,3";
                topOrder = "0,1";
                bottomOrder = "0,1";
                return;
            }


            if (isRandom)
            {
                // Randomize parts vertices for variety
                partTopVertices = Random.Range(.3f, .4f);
                partBottomVertices = -Mathf.Abs(partTopVertices);
                partRightVertices = Random.Range(baseHorizontalLenght / 2.5f, baseHorizontalLenght / 1.65f);
                partLeftVertices = -Mathf.Abs(baseHorizontalLenght - partRightVertices);
            }

            // Calculate material tiling for prevent texture stretching
            topMat[randTopMat].SetTextureScale("_MainTex", new Vector2(1f, totalVerticalLenght / 2));
            bottomMat[randBottomMat].SetTextureScale("_MainTex", new Vector2(totalHorizontalLenght / 2 * 1.3f, 1.3f));

            // Get left order first char and convert to int, get right order last char and convert to int to find out if they have same order
            if (int.Parse(leftOrder.Substring(0, 1)) == 0 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 1
                || int.Parse(leftOrder.Substring(0, 1)) == 1 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 0)
            {
                leftOrder = "0,1";
                rightOrder = "0,1";
                topOrder = "2,3";
                bottomOrder = "2,3";
            }
            else if (int.Parse(leftOrder.Substring(0, 1)) == 0 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 3
                || int.Parse(leftOrder.Substring(0, 1)) == 3 && int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) == 0)
            {
                topOrder = "1,2";
                bottomOrder = "1,2";
            }
        }

        private void CreateMeshComponents(Transform parent, string name, Material material, Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            GameObject mesh = new GameObject(name);
            mesh.transform.parent = parent.transform;

            MeshFilter meshFilter;
            MeshRenderer meshRenderer;

            meshRenderer = mesh.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            meshFilter = mesh.gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.name = name;
            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.uv = uvs;
            meshFilter.sharedMesh.RecalculateNormals();
            meshFilter.sharedMesh.RecalculateBounds();

            mesh.transform.localPosition = Vector3.zero;
        }

        private void CreatePart(GameObject levelParent, string name, Parts part)
        {
            GameObject partParent = new GameObject(name + " Parent");
            partParent.transform.parent = levelParent.transform;

            GameObject meshesParent = new GameObject("Meshes");
            meshesParent.transform.parent = partParent.transform;

            //Top Mesh
            CreateMeshComponents(meshesParent.transform
                , "Top"
                , topMat[randTopMat]
                , GetVertices(partParent.transform, part)
                , proceduralPart.GetTriangles(false)
                , GetUvs(part));

            //Buttom Mesh
            CreateMeshComponents(meshesParent.transform
                , "Buttom"
                , bottomMat[randBottomMat]
                , GetVertices(partParent.transform, part)
                , proceduralPart.GetTriangles(true)
                , GetUvs(part));

            SetComponents(partParent.transform, part);

        }

        private void AddMeshCollider(Transform parent)
        {
            Transform meshes = parent.transform.Find("Meshes");
            meshes.GetChild(0).gameObject.AddComponent<MeshCollider>().convex = true;
            meshes.GetChild(1).gameObject.AddComponent<MeshCollider>().convex = true;
        }

        private void SetComponents(Transform parent, Parts part)
        {
            switch (part)
            {
                case Parts.BASE:
                    LevelController levelController = parent.root.gameObject.AddComponent<LevelController>();

                    levelController.confettiParticles = confettiParticles;
                    levelController.solidBlackMat = solidBlackMat;

                    levelController.soundManager = soundManager;
                    levelController.levelCreator = this;
                    levelController.stickerClip = stickerClip;
                    levelController.winClip = winClip;

                    levelController.rotationCurve = animationCurve;

                    GameObject mainStickerGO = Instantiate(StickerPrefab);
                    mainStickerGO.transform.parent = parent;
                    mainStickerGO.GetComponent<SpriteRenderer>().sprite = stickers[randSticker];
                    mainStickerGO.name = "Main Sticker";

                    GameObject popupSticker = Instantiate(StickerPrefab);
                    popupSticker.transform.parent = parent;
                    popupSticker.GetComponent<SpriteRenderer>().sprite = stickers[randSticker];
                    popupSticker.GetComponent<SpriteRenderer>().material = outlineMat;
                    popupSticker.name = "Popup Sticker";
                    popupSticker.transform.localPosition = new Vector3(popupSticker.transform.position.x, 0.05f, popupSticker.transform.position.z);
                    popupSticker.SetActive(false);

                    levelController.popupSticker = popupSticker;
                    break;
                case Parts.LEFT:
                    leftPart = parent.gameObject;
                    AddMeshCollider(parent);
                    AddSpriteAndMask(parent, part);
                    DashedLine(parent, part);
                    AddFoldPaperScript(parent.gameObject, new Vector3(0, 0, 1), -180f);
                    break;
                case Parts.RIGHT:
                    rightPart = parent.gameObject;
                    AddMeshCollider(parent);
                    AddSpriteAndMask(parent, part);
                    DashedLine(parent, part);
                    AddFoldPaperScript(parent.gameObject, new Vector3(0, 0, 1), 180f);
                    break;
                case Parts.TOP:
                    topPart = parent.gameObject;
                    AddMeshCollider(parent);
                    AddSpriteAndMask(parent, part);
                    DashedLine(parent, part);
                    AddFoldPaperScript(parent.gameObject, new Vector3(1, 0, 0), -180f);
                    break;
                case Parts.BOTTOM:
                    bottomPart = parent.gameObject;
                    AddMeshCollider(parent);
                    AddSpriteAndMask(parent, part);
                    DashedLine(parent, part);
                    AddFoldPaperScript(parent.gameObject, new Vector3(1, 0, 0), 180f);
                    break;
                case Parts.TOP_LEFT:
                    DashedLine(parent, part);
                    SetCorners(parent, new GameObject[] { topPart, leftPart });
                    break;
                case Parts.TOP_RIGHT:
                    DashedLine(parent, part);
                    SetCorners(parent, new GameObject[] { topPart, rightPart });
                    break;
                case Parts.BOTTOM_LEFT:
                    DashedLine(parent, part);
                    SetCorners(parent, new GameObject[] { bottomPart, leftPart });
                    break;
                case Parts.BOTTOM_RIGHT:
                    DashedLine(parent, part);
                    SetCorners(parent, new GameObject[] { bottomPart, rightPart });
                    break;
            }
        }

        private void AddFoldPaperScript(GameObject parent, Vector3 rotationAxis, float angleChange)
        {
            FoldPaper foldPaper = parent.AddComponent<FoldPaper>();

            foldPaper.levelControllerRef = foldPaper.transform.root.GetComponent<LevelController>();

            // Increase number of rotations to calculating how many folding we could
            foldPaper.levelControllerRef.maxNumberOfRotations += 1;

            foldPaper.rotAxis = rotationAxis;
            foldPaper.angleChange = angleChange;

            // Adding right orders to fold parts to track if player folded right or wrong
            if (parent == leftPart)
            {
                for (int i = 0; i < leftOrder.Length; i += 2)
                {
                    foldPaper.rotationOrderIndexes.Add(int.Parse(leftOrder.Substring(i, 1)));
                }
            }
            else if (parent == rightPart)
            {
                for (int i = 0; i < rightOrder.Length; i += 2)
                {
                    foldPaper.rotationOrderIndexes.Add(int.Parse(rightOrder.Substring(i, 1)));
                }
            }
            else if (parent == topPart)
            {
                for (int i = 0; i < topOrder.Length; i += 2)
                {
                    foldPaper.rotationOrderIndexes.Add(int.Parse(topOrder.Substring(i, 1)));
                }
            }
            else if (parent == bottomPart)
            {
                for (int i = 0; i < bottomOrder.Length; i += 2)
                {
                    foldPaper.rotationOrderIndexes.Add(int.Parse(bottomOrder.Substring(i, 1)));
                }
            }
        }

        public void SetCorners(Transform parent, GameObject[] corners)
        {
            // Set corners to fold parts if they intersect

            foreach (GameObject corner in corners)
            {
                FoldPaper foldPaper = corner.GetComponent<FoldPaper>();
                foldPaper.corners.Add(parent.gameObject);

                if (corner == leftPart || corner == rightPart)
                {
                    foldPaper.backCorners.Add(parent.GetChild(1).GetChild(2).gameObject);
                    foldPaper.backCorners.Add(parent.GetChild(1).GetChild(3).gameObject);
                }
                else
                {
                    foldPaper.backCorners.Add(parent.GetChild(1).GetChild(2).gameObject);
                    foldPaper.backCorners.Add(parent.GetChild(1).GetChild(3).gameObject);

                    if (foldPaper.backCorners.Count > 2)
                    {
                        foldPaper.backCorners.Reverse();
                    }
                }
            }
        }

        private void DashedLine(Transform parent, Parts part)
        {
            // Calculate each inner edge to spawn dashed line

            Vector3[] vertices = GetVertices(parent, part);

            GameObject dashedLines = new GameObject("Dashed Lines");
            dashedLines.transform.parent = parent;
            dashedLines.transform.localPosition = Vector3.zero;

            switch (part)
            {
                case Parts.LEFT:
                    CreateDashedLine(new Vector3(vertices[2].x - 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                        new Vector3(90, -90, 0),
                        new Vector2((vertices[1].z + -vertices[0].z) * 10, .4f),
                        dashedLines.transform);
                    break;
                case Parts.RIGHT:
                    CreateDashedLine(new Vector3(vertices[0].x + 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                       new Vector3(90, -90, 180),
                       new Vector2((vertices[1].z + -vertices[0].z) * 10, .4f),
                       dashedLines.transform);
                    break;
                case Parts.TOP:
                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[0].z + 0.02f),
                      new Vector3(90, 0, 0),
                      new Vector2((vertices[2].x + -vertices[0].x) * 10, .4f),
                      dashedLines.transform);
                    break;
                case Parts.BOTTOM:
                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[1].z - 0.02f),
                     new Vector3(90, 0, 180),
                     new Vector2((vertices[2].x + -vertices[0].x) * 10, .4f),
                     dashedLines.transform);
                    break;
                case Parts.TOP_LEFT:
                    CreateDashedLine(new Vector3(vertices[2].x - 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 0),
                    new Vector2(vertices[1].z * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[0].z + 0.02f),
                    new Vector3(90, 0, 0),
                    new Vector2(vertices[0].x * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3(vertices[2].x - 0.02f, -0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 180),
                    new Vector2(vertices[1].z * 10, .4f),
                    dashedLines.transform, false);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, -0.0005f, vertices[0].z + 0.02f),
                    new Vector3(90, 0, 180),
                    new Vector2(vertices[0].x * 10, .4f),
                    dashedLines.transform, false);
                    break;
                case Parts.TOP_RIGHT:
                    CreateDashedLine(new Vector3(vertices[0].x + 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 180),
                    new Vector2(vertices[1].z * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[0].z + 0.02f),
                    new Vector3(90, 0, 0),
                    new Vector2(vertices[2].x * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3(vertices[0].x + 0.02f, -0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 0),
                    new Vector2(vertices[1].z * 10, .4f),
                    dashedLines.transform, false);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, -0.0005f, vertices[0].z + 0.02f),
                    new Vector3(90, 0, 180),
                    new Vector2(vertices[2].x * 10, .4f),
                    dashedLines.transform, false);
                    break;
                case Parts.BOTTOM_LEFT:
                    CreateDashedLine(new Vector3(vertices[2].x - 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 0),
                    new Vector2(vertices[0].z * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[1].z - 0.02f),
                    new Vector3(90, 0, 180),
                    new Vector2(vertices[1].x * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3(vertices[2].x - 0.02f, -0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 180),
                    new Vector2(vertices[0].z * 10, .4f),
                    dashedLines.transform, false);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, -0.0005f, vertices[1].z - 0.02f),
                    new Vector3(90, 0, 0),
                    new Vector2(vertices[1].x * 10, .4f),
                    dashedLines.transform, false);
                    break;
                case Parts.BOTTOM_RIGHT:
                    CreateDashedLine(new Vector3(vertices[0].x + 0.02f, 0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 180),
                    new Vector2(vertices[0].z * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, 0.0005f, vertices[3].z - 0.02f),
                    new Vector3(90, 0, 180),
                    new Vector2(vertices[3].x * 10, .4f),
                    dashedLines.transform);

                    CreateDashedLine(new Vector3(vertices[0].x + 0.02f, -0.0005f, (vertices[1].z + vertices[0].z) / 2),
                    new Vector3(90, -90, 0),
                    new Vector2(vertices[0].z * 10, .4f),
                    dashedLines.transform, false);

                    CreateDashedLine(new Vector3((vertices[0].x + vertices[2].x) / 2, -0.0005f, vertices[3].z - 0.02f),
                    new Vector3(90, 0, 0),
                    new Vector2(vertices[3].x * 10, .4f),
                    dashedLines.transform, false);
                    break;
            }
        }

        private void CreateDashedLine(Vector3 localPosition, Vector3 rotation, Vector2 size, Transform parent, bool isActive = true)
        {
            GameObject dashedLineGO;
            dashedLineGO = Instantiate(dashedLine);
            dashedLineGO.transform.parent = parent;
            dashedLineGO.transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
            dashedLineGO.transform.rotation = Quaternion.Euler(rotation);
            dashedLineGO.GetComponent<SpriteRenderer>().size = new Vector2(size.x, size.y);
            dashedLineGO.SetActive(isActive);
        }

        private void AddSpriteAndMask(Transform parent, Parts part)
        {
            // Calculate sprite and mask positions and scales to make them match orders

            Vector3[] vertices = GetVertices(parent, part);

            GameObject sortingGroupGO = new GameObject("Sorting Group");
            sortingGroupGO.transform.parent = parent;
            sortingGroupGO.transform.localPosition = Vector3.zero;
            sortingGroupGO.AddComponent<SortingGroup>().sortingLayerName = "Sticker";

            GameObject spriteRendererGO = new GameObject("Sprite Renderer");
            spriteRendererGO.transform.parent = sortingGroupGO.transform;
            spriteRendererGO.transform.localScale = new Vector3(.25f, .25f, .25f);

            SpriteRenderer spriteRenderer = spriteRendererGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = stickers[randSticker];
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            spriteRenderer.sortingLayerName = "Sticker";

            GameObject maskGO = new GameObject("Sprite Mask");
            maskGO.transform.parent = sortingGroupGO.transform;

            SpriteMask mask = maskGO.AddComponent<SpriteMask>();
            mask.sprite = maskSprite;
            maskGO.transform.localRotation = Quaternion.Euler(90, 0, 0);
            maskGO.transform.localPosition = new Vector3(maskGO.transform.position.x, -0.001f, maskGO.transform.position.z);

            parent.root.GetComponent<LevelController>().spriteRenderers.Add(spriteRenderer);

            switch (part)
            {
                case Parts.LEFT:

                    // Adjust position and scale to fit meshes and Downscale masks to prevent do not overlap each other

                    maskGO.transform.localPosition =
                        new Vector3(vertices[0].x / 2
                        , maskGO.transform.localPosition.y
                        , (vertices[1].z + vertices[0].z) / 2);

                    maskGO.transform.localScale =
                        new Vector3(vertices[0].x * -1
                        , vertices[1].z + -vertices[0].z
                        , maskGO.transform.localScale.z);

                    // if Top order greater than Left order
                    if (int.Parse(topOrder.Substring(topOrder.Length - 1, 1)) > int.Parse(leftOrder.Substring(leftOrder.Length - 1, 1)))
                    {
                        // Downscale masks to prevent do not overlap each other
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z + (partTopVertices * -1 / 2));

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x
                            , maskGO.transform.localScale.y - partTopVertices
                            , maskGO.transform.localScale.z);
                    }

                    if (int.Parse(bottomOrder.Substring(bottomOrder.Length - 1, 1)) > int.Parse(leftOrder.Substring(leftOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z - (partBottomVertices / 2));

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x
                            , maskGO.transform.localScale.y + partBottomVertices
                            , maskGO.transform.localScale.z);
                    }

                    spriteRendererGO.transform.localPosition = new Vector3((baseLeftVertices + -.5f), -0.001f, .5f);
                    spriteRendererGO.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                    break;
                case Parts.RIGHT:

                    maskGO.transform.localPosition =
                        new Vector3(vertices[2].x / 2
                        , maskGO.transform.localPosition.y
                        , (vertices[2].z + vertices[3].z) / 2);

                    maskGO.transform.localScale =
                        new Vector3(vertices[2].x * -1
                        , vertices[2].z + -vertices[3].z
                        , maskGO.transform.localScale.z);

                    if (int.Parse(topOrder.Substring(topOrder.Length - 1, 1)) > int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z + (partTopVertices * -1 / 2));

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x
                            , maskGO.transform.localScale.y + partTopVertices
                            , maskGO.transform.localScale.z);
                    }

                    if (int.Parse(bottomOrder.Substring(bottomOrder.Length - 1, 1)) > int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z - (partBottomVertices / 2));

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x
                            , maskGO.transform.localScale.y - partBottomVertices
                            , maskGO.transform.localScale.z);
                    }

                    spriteRendererGO.transform.localPosition = new Vector3((baseRightVertices - 1) + .5f, -0.001f, .5f);
                    spriteRendererGO.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                    break;
                case Parts.TOP:

                    maskGO.transform.localPosition =
                       new Vector3((vertices[1].x + vertices[2].x) / 2
                       , maskGO.transform.localPosition.y
                       , vertices[1].z / 2);


                    maskGO.transform.localScale =
                        new Vector3((vertices[1].x * -1 + vertices[2].x)
                        , vertices[1].z * -1
                        , maskGO.transform.localScale.z);

                    if (int.Parse(leftOrder.Substring(leftOrder.Length - 1, 1)) > int.Parse(topOrder.Substring(topOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x - (partLeftVertices / 2)
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z);

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x + partLeftVertices
                            , maskGO.transform.localScale.y
                            , maskGO.transform.localScale.z);
                    }

                    if (int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) > int.Parse(topOrder.Substring(topOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x + (partRightVertices * -1 / 2)
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z);

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x - partRightVertices
                            , maskGO.transform.localScale.y
                            , maskGO.transform.localScale.z);
                    }

                    spriteRendererGO.transform.localPosition = new Vector3(.5f, -0.001f, (baseTopVertices - 1) + .5f);
                    spriteRendererGO.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    break;
                case Parts.BOTTOM:

                    maskGO.transform.localPosition =
                        new Vector3((vertices[1].x + vertices[2].x) / 2
                        , maskGO.transform.localPosition.y
                        , vertices[0].z / 2);

                    maskGO.transform.localScale =
                        new Vector3(vertices[1].x * -1 + vertices[2].x
                        , vertices[0].z * -1
                        , maskGO.transform.localScale.z);

                    if (int.Parse(leftOrder.Substring(leftOrder.Length - 1, 1)) > int.Parse(bottomOrder.Substring(bottomOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x - (partLeftVertices / 2)
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z);

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x + partLeftVertices
                            , maskGO.transform.localScale.y
                            , maskGO.transform.localScale.z);
                    }

                    if (int.Parse(rightOrder.Substring(rightOrder.Length - 1, 1)) > int.Parse(bottomOrder.Substring(bottomOrder.Length - 1, 1)))
                    {
                        maskGO.transform.localPosition =
                            new Vector3(maskGO.transform.localPosition.x + (partRightVertices * -1 / 2)
                            , maskGO.transform.localPosition.y
                            , maskGO.transform.localPosition.z);

                        maskGO.transform.localScale =
                            new Vector3(maskGO.transform.localScale.x - partRightVertices
                            , maskGO.transform.localScale.y
                            , maskGO.transform.localScale.z);
                    }

                    spriteRendererGO.transform.localPosition = new Vector3(.5f, -0.001f, (baseBottomVertices + -.5f));
                    spriteRendererGO.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    break;
            }
        }

        private Vector3[] GetVertices(Transform parent, Parts part)
        {
            return proceduralPart.GetVertices(parent.transform, part, baseLeftVertices, baseRightVertices, baseTopVertices, baseBottomVertices
                , partLeftVertices, partRightVertices, partTopVertices, partBottomVertices);
        }

        private Vector2[] GetUvs(Parts part)
        {
            return proceduralPart.GetUvs(part, baseLeftVertices, baseRightVertices, baseTopVertices, baseBottomVertices
                , partLeftVertices, partRightVertices, partTopVertices, partBottomVertices);
        }

        public void DeleteLevels()
        {
            if (levels.Count > 0)
            {
                DestroyImmediate(levels[levels.Count - 1]);
                levels.RemoveAt(levels.Count - 1);
            }
        }

        public void DeleteAllLevels()
        {
            if (levels.Count > 0)
            {
                foreach (GameObject item in levels)
                {
                    DestroyImmediate(item);
                }
                levels.Clear();
            }
        }

    }
}