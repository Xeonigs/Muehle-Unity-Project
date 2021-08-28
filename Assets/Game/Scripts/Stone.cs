using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public GameTable.WhoseMove color;

    private GameTable gameTable;
    private bool isDragging = false;
    private Vector2 startPosition;
    private bool isOverPlace = false;
    private GameObject hoveringPlace = null;

    public enum VerticalPosition { None, Up,  Middle, Down };
    public enum HorizontalPosition { None, Left, Middle, Right };

    private VerticalPosition verticalPosition = VerticalPosition.None;
    private HorizontalPosition horizontalPosition = HorizontalPosition.None;

    private void Start()
    {
        gameTable = transform.parent.parent.GetComponent<GameTable>();
    }

    public void ClickStone()
    {
        gameTable.ClickedStone(gameObject);
    }

    public void StartDragging()
    {
        if (gameTable.CanMove(color, transform.parent.gameObject))
        {
            startPosition = transform.position;
            gameTable.ShowAllPossibleMoves(gameObject);
            isDragging = true;
        }
    }

    public void StopDragging()
    {
        if (isDragging)
        {
            gameTable.HideAllPossibleMoves();
            isDragging = false;
            if (isOverPlace && gameTable.CanPlace(transform.parent.gameObject, hoveringPlace))
            {
                transform.SetParent(hoveringPlace.transform, false);

                if (transform.parent.GetComponent<Place>().down == null)
                {
                    verticalPosition = VerticalPosition.Down;
                }
                else if (transform.parent.GetComponent<Place>().up == null)
                {
                    verticalPosition = VerticalPosition.Up;
                }
                else
                {
                    verticalPosition = VerticalPosition.Middle;
                }

                if (transform.parent.GetComponent<Place>().left == null)
                {
                    horizontalPosition = HorizontalPosition.Left;
                }
                else if (transform.parent.GetComponent<Place>().right == null)
                {
                    horizontalPosition = HorizontalPosition.Right;
                }
                else
                {
                    horizontalPosition = HorizontalPosition.Middle;
                }

                transform.position = hoveringPlace.transform.position;
                gameTable.TableChanged(gameObject);
            }
            else
            {
                transform.position = startPosition;
            }
            isOverPlace = false;
        }
    }

    public VerticalPosition GetVerticalPosition()
    {
        return verticalPosition;
    }

    public HorizontalPosition GetHorizontalPosition()
    {
        return horizontalPosition;
    }

    // Aufpassen bei den Collisions.
    // Wenn man ein Place verlässt, während man einen anderen Place schon betreten hat, ist hoveringPlace auf null und darum kann es zu Komplikationen und Bugs führen. 
    // Deshalb habe ich den Radius des Colliders auf das Minimale reduziert und somit nur einen Punkt als Collider, damit nicht solche komischen Übergänge stattfinden können.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Place"))
        {
            isOverPlace = true;
            hoveringPlace = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Place"))
        {
            isOverPlace = false;
            hoveringPlace = null;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}