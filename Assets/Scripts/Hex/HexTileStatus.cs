namespace RotatingRoutes.Hex
{
    [System.Serializable]
    public struct HexTileStatus
    {
        public HexTileType Type;
        public float RotationAngle;
        public int ModelId;
        public int ScaleY;

        public HexTileStatus(HexTileType type, float rotationAngle, int modelId = -1, int scaleY = 1)
        {
            Type = type;
            RotationAngle = rotationAngle;
            ModelId = modelId;
            ScaleY = scaleY;
        }

        public override string ToString()
        {
            return $"Status: {Type} angle: {RotationAngle} model: {ModelId} scale  {ScaleY}";
        }
    }
}