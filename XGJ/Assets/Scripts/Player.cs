using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float money;

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

    private Vector3 originalPos;

    public float speed;
    public float turnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        movingMachine = false;
        chosenMachine = 0;
        isPlacing = false;
        UpdateText();
        GenerateStore();
    }

    void Update()
    {
        if (!isPlacing)
        {
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                transform.Translate(localForward * 2f, Space.Self);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                transform.Translate(-localForward * 2f, Space.Self);
            }


            if (Input.GetMouseButton(2))
            {
                if (Input.GetAxis("Mouse X") > 0f)
                {
                    Vector3 flatRight = transform.right;
                    flatRight.y = 0;
                    transform.Translate(-flatRight * Input.GetAxis("Mouse X") * speed * 2 * Time.fixedDeltaTime, Space.World);
                }
                else if (Input.GetAxis("Mouse X") < 0f)
                {
                    Vector3 flatRight = transform.right;
                    flatRight.y = 0;
                    transform.Translate(-flatRight * Input.GetAxis("Mouse X") * speed * 2 * Time.fixedDeltaTime, Space.World);
                }

                if (Input.GetAxis("Mouse Y") > 0f)
                {
                    Vector3 flatForward = transform.forward;
                    flatForward.y = 0;
                    transform.Translate(-flatForward * Input.GetAxis("Mouse Y") * speed * 2 * Time.fixedDeltaTime, Space.World);
                }
                else if (Input.GetAxis("Mouse Y") < 0f)
                {
                    Vector3 flatForward = transform.forward;
                    flatForward.y = 0;
                    transform.Translate(-flatForward * Input.GetAxis("Mouse Y") * speed * 2 * Time.fixedDeltaTime, Space.World);
                }
            }
            else if (Input.GetMouseButton(1))
            {
                if (Input.GetAxis("Mouse X") > 0f)
                {
                    transform.Rotate(-Vector3.down * Input.GetAxis("Mouse X") * turnSpeed * Time.fixedDeltaTime, Space.World);
                }
                else if (Input.GetAxis("Mouse X") < 0f)
                {
                    transform.Rotate(-Vector3.down * Input.GetAxis("Mouse X") * turnSpeed * Time.fixedDeltaTime, Space.World);
                }
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 flatRight = transform.right;
            flatRight.y = 0;
            transform.Translate(-flatRight * speed * Time.fixedDeltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 flatRight = transform.right;
            flatRight.y = 0;
            transform.Translate(flatRight * speed * Time.fixedDeltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0;
            transform.Translate(-flatForward * speed * Time.fixedDeltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0;
            transform.Translate(flatForward * speed * Time.fixedDeltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.down * turnSpeed * Time.fixedDeltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * turnSpeed * Time.fixedDeltaTime, Space.World);
        }

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
        }
        else if (isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Physics.Raycast(transform.position, ray.direction, out hit, float.MaxValue, LayerMask.GetMask("Floor"));

            Vector3 newPos = new Vector3(RoundToNearestHalf(hit.point.x), RoundToNearestHalf(hit.point.y), RoundToNearestHalf(hit.point.z));

            // Debug.Log(newPos);

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
                    if (money >= arcadeMachine[chosenMachine].GetComponent<ArcadeMachine>().machinePurchasePrice)
                    {
                        money -= arcadeMachine[chosenMachine].GetComponent<ArcadeMachine>().machinePurchasePrice;
                        UpdateText();
                        GameObject go = Instantiate(arcadeMachine[chosenMachine], placingObject.transform.position, placingObject.transform.rotation);
                        go.GetComponent<ArcadeMachine>().UpdateInfo();
                    }
                    else
                    {
                        Debug.Log("NOT ENOUGH MONEYYSS!!!!");
                    }
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

    public void PayRent()
    {
        money -= (arcade.rent + arcade.electricity);

        if (money <= 0)
        {
            Debug.Log("you lost the game :(((((");
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
            TextMeshProUGUI[] textTable = gen.GetComponentsInChildren<TextMeshProUGUI>();
            for (int z = 0; z < textTable.Length; z++)
            {
                switch(textTable[z].name)
                {
                    case "machinename":
                        textTable[z].text = am.machineName;
                        break;
                    case "machineprice":
                        textTable[z].text = "$ " + am.machinePurchasePrice.ToString();
                        break;
                    default:
                        break;
                }
            }
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
