using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 30;
    public int height = 20;
    public GameObject cellPrefab;
    public Transform gridParent;

    [Header("Simulation Settings")]
    [Range(0.05f, 1f)]
    public float updateDelay = 0.3f; // задержка между поколениями

    private CellController[,] cells;
    private bool isRunning = false;
    private Coroutine simulationCoroutine;

    void Start()
    {
        GenerateGrid();
    }

    // ==========================================================
    //                 СОЗДАНИЕ СЕТКИ КЛЕТОК
    // ==========================================================
    void GenerateGrid()
    {
        cells = new CellController[width, height];

        // ВАЖНО: переворачиваем цикл Y, чтобы сетка совпадала с визуальным порядком
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cellGO = Instantiate(cellPrefab, gridParent);
                CellController cell = cellGO.GetComponent<CellController>();
                cells[x, y] = cell;

                int cx = x;
                int cy = y;
                cellGO.GetComponent<Button>().onClick.AddListener(() => cells[cx, cy].Toggle());
            }
        }
    }

    // ==========================================================
    //                    ЗАПУСК / ПАУЗА
    // ==========================================================
    public void ToggleRun()
    {
        isRunning = !isRunning;

        if (isRunning)
            simulationCoroutine = StartCoroutine(RunSimulation());
        else if (simulationCoroutine != null)
            StopCoroutine(simulationCoroutine);
    }

    IEnumerator RunSimulation()
    {
        while (isRunning)
        {
            NextGeneration();
            yield return new WaitForSeconds(updateDelay);
        }
    }

    // ==========================================================
    //                  ОСНОВНАЯ СИМУЛЯЦИЯ
    // ==========================================================
    void NextGeneration()
    {
        bool[,] newState = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbors = CountAliveNeighbors(x, y);
                bool alive = cells[x, y].isAlive;

                // Правила Конвея
                if (alive && (neighbors == 2 || neighbors == 3))
                    newState[x, y] = true;
                else if (!alive && neighbors == 3)
                    newState[x, y] = true;
                else
                    newState[x, y] = false;
            }
        }

        // Обновляем состояние и цвет
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells[x, y].SetAlive(newState[x, y]);
    }

    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (cells[nx, ny].isAlive)
                        count++;
                }
            }
        }

        return count;
    }

    // ==========================================================
    //                     УПРАВЛЕНИЕ
    // ==========================================================
    public void Randomize()
    {
        foreach (var cell in cells)
            cell.SetAlive(Random.value > 0.7f);
    }

    public void ClearGrid()
    {
        foreach (var cell in cells)
            cell.SetAlive(false);
    }

    public void SetSpeed(float value)
    {
        // ограничим диапазон от 0.05 сек (быстро) до 1 сек (медленно)
        updateDelay = Mathf.Lerp(1f, 0.05f, (value - 1f) / 9f);
    }

}
