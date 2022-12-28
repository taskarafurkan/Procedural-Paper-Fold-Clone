using UnityEngine;

namespace PaperFold.ProcGen
{
    public enum Parts
    {
        BASE, LEFT, RIGHT, TOP, BOTTOM, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT,
    }

    public class ProceduralPart
    {
        public Vector3[] GetVertices(Transform parent, Parts part, float baseLeftVertices, float baseRightVertices, float baseTopVertices, float baseBottomVertices
            , float partLeftVertices, float partRightVertices, float partTopVertices, float partBottomVertices)
        {
            switch (part)
            {
                case Parts.BASE:
                    return SetVertices(baseLeftVertices, baseRightVertices, baseBottomVertices, baseTopVertices);
                case Parts.LEFT:
                    parent.localPosition = new Vector3(baseLeftVertices, parent.position.y, parent.position.z);
                    return SetVertices(partLeftVertices, 0, baseBottomVertices, baseTopVertices);
                case Parts.RIGHT:
                    parent.localPosition = new Vector3(baseRightVertices, parent.position.y, parent.position.z);
                    return SetVertices(0, partRightVertices, baseBottomVertices, baseTopVertices);
                case Parts.TOP:
                    parent.localPosition = new Vector3(parent.position.x, parent.position.y, baseTopVertices);
                    return SetVertices(baseLeftVertices, baseRightVertices, 0, partTopVertices);
                case Parts.BOTTOM:
                    parent.localPosition = new Vector3(parent.position.x, parent.position.y, baseBottomVertices);
                    return SetVertices(baseLeftVertices, baseRightVertices, partBottomVertices, 0);
                case Parts.TOP_LEFT:
                    parent.localPosition = new Vector3(baseLeftVertices, parent.position.y, baseTopVertices);
                    return SetVertices(partLeftVertices, 0, baseTopVertices - parent.position.z, partTopVertices);
                case Parts.TOP_RIGHT:
                    parent.localPosition = new Vector3(baseRightVertices, parent.position.y, baseTopVertices);
                    return SetVertices(0, partRightVertices, baseTopVertices - parent.position.z, partTopVertices);
                case Parts.BOTTOM_LEFT:
                    parent.localPosition = new Vector3(baseLeftVertices, parent.position.y, baseBottomVertices);
                    return SetVertices(partLeftVertices, 0, partBottomVertices, 0);
                case Parts.BOTTOM_RIGHT:
                    parent.localPosition = new Vector3(baseRightVertices, parent.position.y, baseBottomVertices);
                    return SetVertices(0, partRightVertices, partBottomVertices, 0);
            }
            return null;
        }

        public Vector2[] GetUvs(Parts part, float baseLeftVertices, float baseRightVertices, float baseTopVertices, float baseBottomVertices
            , float leftVerticesX, float rightVerticesX, float topVerticesZ, float buttomVerticesZ)
        {
            float verticalLenght = Mathf.Abs(buttomVerticesZ) + topVerticesZ + Mathf.Abs(baseBottomVertices) + baseTopVertices;
            float horizontalLenght = Mathf.Abs(leftVerticesX) + rightVerticesX + Mathf.Abs(baseLeftVertices) + baseRightVertices;

            switch (part)
            {
                case Parts.BASE:
                    return SetUvs((Mathf.Abs(leftVerticesX) / horizontalLenght), 1 - (rightVerticesX / horizontalLenght), 1 - (topVerticesZ / verticalLenght), (Mathf.Abs(buttomVerticesZ) / verticalLenght));
                case Parts.LEFT:
                    return SetUvs(baseLeftVertices, (Mathf.Abs(leftVerticesX) / horizontalLenght), 1 - (topVerticesZ / verticalLenght), (Mathf.Abs(buttomVerticesZ) / verticalLenght));
                case Parts.RIGHT:
                    return SetUvs(1 - (rightVerticesX / horizontalLenght), baseRightVertices, 1 - (topVerticesZ / verticalLenght), (Mathf.Abs(buttomVerticesZ) / verticalLenght));
                case Parts.TOP:
                    return SetUvs((Mathf.Abs(leftVerticesX) / horizontalLenght), 1 - (rightVerticesX / horizontalLenght), baseTopVertices, 1 - (topVerticesZ / verticalLenght));
                case Parts.BOTTOM:
                    return SetUvs((Mathf.Abs(leftVerticesX) / horizontalLenght), 1 - (rightVerticesX / horizontalLenght), (Mathf.Abs(buttomVerticesZ) / verticalLenght), baseBottomVertices);
                case Parts.TOP_LEFT:
                    return SetUvs(baseLeftVertices, (Mathf.Abs(leftVerticesX) / horizontalLenght), baseTopVertices, 1 - (topVerticesZ / verticalLenght));
                case Parts.TOP_RIGHT:
                    return SetUvs(1 - (rightVerticesX / horizontalLenght), baseRightVertices, baseRightVertices, 1 - (topVerticesZ / verticalLenght));
                case Parts.BOTTOM_LEFT:
                    return SetUvs(baseLeftVertices, (Mathf.Abs(leftVerticesX) / horizontalLenght), (Mathf.Abs(buttomVerticesZ) / verticalLenght), baseBottomVertices);
                case Parts.BOTTOM_RIGHT:
                    return SetUvs(1 - (rightVerticesX / horizontalLenght), baseRightVertices, (Mathf.Abs(buttomVerticesZ) / verticalLenght), baseBottomVertices);
            }
            return null;
        }

        public int[] GetTriangles(bool isButtom)
        {
            if (!isButtom)
            {
                return new int[] { 0, 1, 2, 1, 3, 2 };
            }

            return new int[] { 0, 2, 1, 1, 2, 3 };
        }

        private Vector3[] SetVertices(float leftX, float rightX, float buttomZ, float topZ)
        {
            Vector3[] vertices =
            {
                new Vector3(leftX, 0, buttomZ),
                new Vector3(leftX, 0, topZ),
                new Vector3(rightX, 0, buttomZ),
                new Vector3(rightX, 0, topZ)
            };

            return vertices;
        }

        private Vector2[] SetUvs(float leftUV, float rightUV, float topUV, float buttomUV)
        {
            Vector2[] uvs =
            {
                new Vector2(leftUV, buttomUV),
                new Vector2(leftUV, topUV),
                new Vector2(rightUV, buttomUV),
                new Vector2(rightUV, topUV)
            };

            return uvs;
        }
    }
}