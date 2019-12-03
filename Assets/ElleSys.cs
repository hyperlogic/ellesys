using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public char lhs;
    public string rhs;

    /*
    public Rule(char lhsIn, string rhsIn) {
        lhs = lhsIn;
        rhs = rhsIn;
    }
    */
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
    public void Turn(float angle) {
        Matrix4x4 top = matStack.Pop();
        top = top * Matrix4x4.Rotate(Quaternion.AngleAxis(angle, Vector3.right));
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
    public GameObject myPrefab;
    public int depth = 1;
    public float posOffset = 1.0f;
    public float anglePlus = 45.0f; // degrees
    public float angleMinus = -45.0f; // degrees

    public string axiom = "0";
    public List<Rule> rules = new List<Rule>();

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
        /*
        // rules for binary tree.
        axiom = "0";
        posOffset = 1.0f;
        anglePlus = 45.0f; // degrees
        angleMinus = -45.0f; // degrees
        rules[0] = new Rule('0', "1[0]0");
        rules[1] = new Rule('1', "11");
        */

        // rules for sierpenski's gasket
        //axiom = "F-G-G";
        //posOffset = 1.0f;
        //anglePlus = 120.0f; // degrees
        //angleMinus = -120.0f; // degrees
        //rules[0] = new Rule('F', "F-G+F+F-F");
        //rules[1] = new Rule('G', "GG");

        string work = axiom;
        for (int i = 0; i < depth; i++) {
            Debug.Log("Index: " + i);
            work = Expand(rules, work);
        }
        Debug.Log("Result: " + work);

        Turtle turtle = new Turtle(transform.position, transform.rotation * Quaternion.AngleAxis(90.0f, Vector3.left));

        foreach (char c in work) {
            /*
            // for binary tree
            if (c == '0' || c == '1') {
                // "draw forward"
                turtle.MoveForward(posOffset);
                GameObject obj = Instantiate(myPrefab, turtle.GetPosition(), turtle.GetRotation());
            } else if (c == '[') {
                // push and turn
                turtle.Push();
                turtle.Turn(pushRotOffset);
            } else if (c == ']') {
                // pop and turn
                turtle.Pop();
                turtle.Turn(popRotOffset);
            }
            */

            if (c == 'F' || c == 'G') {
                // "draw forward"
                turtle.MoveForward(posOffset);
                GameObject obj = Instantiate(myPrefab, turtle.GetPosition(), turtle.GetRotation());
            } else if (c == '+') {
                turtle.Turn(anglePlus);
            } else if (c == '-') {
                turtle.Turn(angleMinus);
            }
        }

        /*
        GameObject prevObj = null;
        bool hasPrevObj = false;
        for (int i = 0; i < depth; i++) {
            // Instantiate at position (0, 0, 0) and zero rotation.
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(pushRotOffset, 0.0f, 0.0f);
            Vector3 translation = new Vector3(0, posOffset, 0);
            Vector3 scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            GameObject obj = Instantiate(myPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (hasPrevObj) {
                obj.transform.SetParent(prevObj.transform);
            }
            obj.transform.localPosition = translation;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = scale;
            prevObj = obj;
            hasPrevObj = true;
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }
}
