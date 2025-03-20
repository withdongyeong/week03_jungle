using UnityEngine;

public class TileFloorGenerator : MonoBehaviour
{
    public GameObject tilePrefab;  // 큐브(타일 프리팹)
    public int rows = 30;  // 바닥의 가로 크기 (행)
    public int columns = 30;  // 바닥의 세로 크기 (열)
    public float tileSize = 30f;  // 타일 크기 (1 x 1 크기)

    void Start()
    {
        GenerateTileFloor();
    }

    void GenerateTileFloor()
    {
        // 1000x1000 크기의 타일 바닥을 만듦
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 각 타일의 위치 계산
                Vector3 position = new Vector3(col * tileSize, 0, row * tileSize);

                // 타일을 생성하여 해당 위치에 배치
                Instantiate(tilePrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
