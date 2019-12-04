using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public char lhs;
    public string rhs;
}

[System.Serializable]
public class Action
{
    public char symbol;
    public float moveZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public bool push;
    public bool pop;
    public bool leaf;
}

public class Turtle
{
    public Stack<Matrix4x4> matStack = new Stack<Matrix4x4>();

    public Turtle(Vector3 posIn, Quaternion rotIn) {
        Matrix4x4 mat = new Matrix4x4();
        mat.SetTRS(posIn, rotIn, Vector3.one);
        matStack.Push(mat);
    }

    // offset is in meters
    public void MoveForward(float offset) {
        Matrix4x4 top = matStack.Pop();
        Vector3 forward = new Vector3(0.0f, 0.0f, offset);
        Matrix4x4 moveForwardMat = new Matrix4x4();
        moveForwardMat.SetTRS(forward, Quaternion.identity, Vector3.one);
        top = top * moveForwardMat;
        matStack.Push(top);
    }

    // angle is in degrees
    public void Turn(Vector3 eulerAngles) {
        Matrix4x4 top = matStack.Pop();
        top = top * Matrix4x4.Rotate(Quaternion.Euler(eulerAngles));
        matStack.Push(top);
    }

    public void Push() {
        Matrix4x4 top = matStack.Pop();
        matStack.Push(top);
        matStack.Push(top);
    }

    public void Pop() {
        matStack.Pop();
    }

    public Vector3 GetPosition() {
        return matStack.Peek().GetColumn(3);
    }

    public Quaternion GetRotation() {
        return matStack.Peek().rotation;
    }
}

public class ElleSys : MonoBehaviour
{
    public GameObject trunkPrefab;
    public GameObject leafPrefab;
    public int depth = 1;

    public string axiom = "0";
    public List<Rule> rules = new List<Rule>();
    public List<Action> actions = new List<Action>();

    // apply rules to work string.
    string Expand(List<Rule> rules, string work) {
        Debug.Log("work = " + work);
        StringBuilder b = new StringBuilder();
        foreach (char c in work) {
            bool foundRule = false;
            foreach (Rule rule in rules) {
                Debug.Log("trying = " + rule.lhs + " -> " + rule.rhs);
                if (rule.lhs == c) {
                    // apply rule
                    b.Append(rule.rhs);
                    foundRule = true;
                    break;
                }
            }
            if (!foundRule) {
                // assume this is a constant.
                b.Append(c);
            }
        }

        Debug.Log("return = " + b.ToString());
        return b.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        string work = axiom;
        for (int i = 0; i < depth; i++) {
            work = Expand(rules, work);
        }

        Turtle turtle = new Turtle(transform.position, transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.left));

        foreach (char c in work) {
            foreach (Action action in actions) {
                if (action.symbol == c) {
                    if (action.pop) {
                        turtle.Pop();
                    }
                    if (action.push) {
                        turtle.Push();
                    }
                    if (action.rotX != 0.0f || action.rotY != 0.0f || action.rotZ != 0.0f) {
                        turtle.Turn(new Vector3(action.rotX, action.rotY, action.rotZ));
                    }
                    if (action.moveZ != 0.0f) {
                        turtle.MoveForward(action.moveZ);
                        if (action.leaf) {
                            Instantiate(leafPrefab, turtle.GetPosition(), turtle.GetRotation());
                        } else {
                            Instantiate(trunkPrefab, turtle.GetPosition(), turtle.GetRotation());
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }
}
