using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace Pathfinding
{
    public class Node : MonoBehaviour, INode<Node>, IPointerDownHandler
    {
        public static event Action<Node> OnClicked = delegate { };

        public Vector2Int GridPosition { get; private set; }
        public NodeDisplay Display => display;

        [SerializeField] LayerMask mask;
        [SerializeField] NodeDisplay display;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked(this);
        }

        public void Setup(Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            name = $"node_({gridPosition.x},{gridPosition.y})";
        }

        //static readonly Vector3[] neighborDirections = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };
        static readonly Vector3[] neighborDirections = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right, new Vector3(1,0,1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1) };
        IEnumerable<(Node node, int cost)> ListNeighbors()
        {
            foreach (var direction in neighborDirections)
            {
                var hit = Physics.Raycast
                (
                    origin: transform.position,
                    direction: direction,
                    //maxDistance: 1f,
                    maxDistance: Mathf.Sqrt(2),
                    layerMask: mask,
                    hitInfo: out var hitInfo
                );

                if (!hit) continue;

                if (hitInfo.collider.TryGetComponent<Node>(out var neighbor))
                    yield return (neighbor, 1);
            }
        }

        public IEnumerable<(Node node, int cost)> Neighbors => ListNeighbors();

        //public int GetHeuristicCostTo(Node target) => Mathf.Abs(GridPosition.x - target.GridPosition.x) + Mathf.Abs(GridPosition.y - target.GridPosition.y);

        //public int GetHeuristicCostTo(Node target) => 1 * Mathf.Max(Mathf.Abs(GridPosition.x - target.GridPosition.x), Mathf.Abs(GridPosition.y - target.GridPosition.y)) + (Mathf.RoundToInt(Mathf.Sqrt(2)) - 1) * Mathf.Min(Mathf.Abs(GridPosition.x - target.GridPosition.x), Mathf.Abs(GridPosition.y - target.GridPosition.y));
        public int GetHeuristicCostTo(Node target) => Mathf.RoundToInt( 1 * Mathf.Max(Mathf.Abs(GridPosition.x - target.GridPosition.x), Mathf.Abs(GridPosition.y - target.GridPosition.y)) + (Mathf.Sqrt(2) - 1) * Mathf.Min(Mathf.Abs(GridPosition.x - target.GridPosition.x), Mathf.Abs(GridPosition.y - target.GridPosition.y)));
    }
}