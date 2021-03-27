using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private float money;

    public Arcade arcade;

    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI storeText;

    public Image shopBgImage;
    public GameObject shopButton;

    public List<GameObject> arcadeMachine;
    private int chosenMachine;

    private bool movingMachine;
    private bool isPlacing;

    private GameObject placingObject;

    // Start is called before the first frame update
    void Start()
    {
        movingMachine = false;
        chosenMachine = 0;
        isPlacing = false;
        money = 100f;
        GenerateStore();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPlacing && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Physics.Raycast(transform.position, ray.direction, out hit);

            Vector3 spawnPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            if (hit.collider.GetComponent<ArcadeMachine>())
            {
                movingMachine = true;
                isPlacing = true;
                placingObject = hit.collider.gameObject;
                placingObject.GetComponent<ArcadeMachine>().ResetUser();
                arcade.DeleteArcadeMachine(placingObject.GetComponent<ArcadeMachine>());
            }
            /*
            else
            {
                isPlacing = true;
                placingObject = Instantiate(arcadeMachine[chosenMachine], spawnPoint, Quaternion.identity);
            }
            */
        }
        else if (isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Physics.Raycast(transform.position, ray.direction, out hit, float.MaxValue, LayerMask.GetMask("Floor"));

            Vector3 newPos = new Vector3(RoundToNearestHalf(hit.point.x), RoundToNearestHalf(hit.point.y), RoundToNearestHalf(hit.point.z));

            Debug.Log(newPos);

            placingObject.transform.position = newPos;

             if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                placingObject.transform.Rotate(Vector3.up * 45f / 2f, Space.Self);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                placingObject.transform.Rotate(Vector3.down * 45f / 2f, Space.Self);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!movingMachine)
                {
                    GameObject go = Instantiate(arcadeMachine[chosenMachine], placingObject.transform.position, placingObject.transform.rotation);
                    go.GetComponent<ArcadeMachine>().UpdateInfo();
                }
                else
                {
                    placingObject.GetComponent<ArcadeMachine>().UpdateInfo();
                    movingMachine = false;
                    isPlacing = false;
                    placingObject = null;
                }
                
            }

            if (Input.GetMouseButtonDown(1))
            {
                Destroy(placingObject);
                isPlacing = false;
                placingObject = null;
            }
        }
    }

    public void ToggleStore()
    {
        if (shopBgImage.gameObject.activeSelf)
        {
            storeText.text = "Store";
            shopBgImage.gameObject.SetActive(false);
        }
        else
        {
            storeText.text = "Close";
            shopBgImage.gameObject.SetActive(true);
        }
    }

    private void GenerateStore()
    {
        for (int j = 0; j < arcadeMachine.Count; j++)
        {
            int k = j;
            ArcadeMachine am = arcadeMachine[j].GetComponent<ArcadeMachine>();

            GameObject gen = Instantiate(shopButton, Vector3.zero, Quaternion.identity, shopBgImage.transform);

            gen.GetComponent<RectTransform>().anchoredPosition = new Vector3 (-320 + (j * 160), 160, 0);
            gen.GetComponentInChildren<TextMeshProUGUI>().text = am.machineName;
            gen.GetComponent<Button>().onClick.AddListener(() => SetChosenMachine(k));
        }

    }

    private float RoundToNearestHalf(float a)
    {
        return Mathf.Round(a * 4f) / 4f;
    }

    public void SetChosenMachine(int number)
    {
        if (isPlacing) return;

        storeText.text = "Store";
        shopBgImage.gameObject.SetActive(false);

        chosenMachine = number;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(transform.position, ray.direction, out hit);

        Vector3 spawnPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        placingObject = Instantiate(arcadeMachine[chosenMachine], spawnPoint, Quaternion.identity);

        isPlacing = true;
    }

    private void UpdateText()
    {
        moneyText.text = "Money : $" + money.ToString();
    }

    public void AddMoney(float amount)
    {
        money += amount;
        UpdateText();
    }

    public bool RemoveMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateText();
            return true;
        }

        return false;
    }

}
