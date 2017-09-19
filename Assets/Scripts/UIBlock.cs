using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBlock : MonoBehaviour {
	//constants defining where the offset for each of the positions are
	private static Vector2 leftOffset, rightOffset, bottomOffset, topOffset;
	//constant defining what the scale should be. The active block (mainScale) will be larger than all others (secondaryScale)
	private static Vector2 mainScale, secondaryScale;

	//how many of these blocks are available for the player to use
	private int _count;
	public int Count {
		get {
			return _count;
		}
		set {
			_count = value;
			text.text = "x"+_count;
		}
	}
	private Sprite _sprite;
	public Sprite Sprite {
		get {
			return image.overrideSprite;
		}
		set {
			_sprite = value;
			image.overrideSprite = _sprite;
		}
	}
	private TextMeshProUGUI text;
	private Image image;
	private RectTransform rt;

	public BlockTypes BlockType { get; set; }

	//where is the block currrently located
	public UIBlockPositions CurrentPosition { get;set; }
	//where does the block have to head next
	public Queue<UIBlockPositions> PositionQueue;
	//what was the most recently added position to the queue--used to find out where the next position in the queue will be
	public UIBlockPositions MostRecentlyQueued;

	//is this block the one that is being used for placement (at the bottom on the UI)
	public bool ActiveBlock;

	//is the Block currently being moved from one placement to another?
	public bool Moving { get; private set; }

	void Awake() {
		text = GetComponentInChildren<TextMeshProUGUI>();
		image = GetComponentInChildren<Image>();
		rt = GetComponent<RectTransform>();

		PositionQueue = new Queue<UIBlockPositions>();

		leftOffset = new Vector2(-45, 15);
		rightOffset = new Vector2(45, 15);
		bottomOffset = new Vector2(0, -22);
		topOffset = new Vector2(0, 50);

		mainScale = Vector2.one;
		secondaryScale = Vector2.one * 0.7f;
	}
	
	void Start () {
		
	}
	void Update () {
			
	}

	public void Init(BlockTypes type, UIBlockPositions p, int count) {
		switch (type)
		{
			default:
			case BlockTypes.Dirt:
				Sprite = Resources.LoadAll<Sprite>("Sprites/Blocks")[2];
				break;
			case BlockTypes.Clay:
				Sprite = Resources.LoadAll<Sprite>("Sprites/Blocks")[1];
				break;
			case BlockTypes.Steel:
				Sprite = Resources.LoadAll<Sprite>("Sprites/Blocks")[0];
				break;
			case BlockTypes.Cloud:
				Sprite = Resources.LoadAll<Sprite>("Sprites/Blocks")[3];
				break;
		}
		BlockType = type;
		Count = count;

		CurrentPosition = p;
		MostRecentlyQueued = p;

		rt.localPosition = GetLocationPositionFromPosition(p);
		rt.transform.localScale = GetScaleFromPosition(p);
	}

	public void SetActiveBlock() {
		ActiveBlock = MostRecentlyQueued == UIBlockPositions.Bottom;
	}

	public void RotateLeft(int blockTypes) {
		//block types is the number of block types in the selector	
		//can only rotate if there is more than 1 block
		if (blockTypes > 1) {
			if (MostRecentlyQueued == UIBlockPositions.Bottom) {
				//new position is left
				PositionQueue.Enqueue(UIBlockPositions.Left);
				MostRecentlyQueued = UIBlockPositions.Left;
			}
			else if(MostRecentlyQueued == UIBlockPositions.Left && blockTypes == 4) {
				//new position is top
				PositionQueue.Enqueue(UIBlockPositions.Top);
				MostRecentlyQueued = UIBlockPositions.Top;

			}
			else if(MostRecentlyQueued == UIBlockPositions.Top || (MostRecentlyQueued == UIBlockPositions.Left && blockTypes == 3)){
				// new position is right
				PositionQueue.Enqueue(UIBlockPositions.Right);
				MostRecentlyQueued = UIBlockPositions.Right;
			}
			else if(MostRecentlyQueued == UIBlockPositions.Right || (MostRecentlyQueued == UIBlockPositions.Left && blockTypes == 2)){
				//new position is bottom
				PositionQueue.Enqueue(UIBlockPositions.Bottom);
				MostRecentlyQueued = UIBlockPositions.Bottom;
			}
			SetActiveBlock();
		}	
	}

	public void RotateRight(int blockTypes) {
		//block types is the number of block types in the selector	
		//can only rotate if there is more than 1 block
		if (blockTypes > 1)
		{
			if (MostRecentlyQueued == UIBlockPositions.Left)
			{
				//new position is bottom
				PositionQueue.Enqueue(UIBlockPositions.Bottom);
				MostRecentlyQueued = UIBlockPositions.Bottom;
			}
			else if (MostRecentlyQueued == UIBlockPositions.Right && blockTypes >= 4)
			{
				//new position is top
				PositionQueue.Enqueue(UIBlockPositions.Top);
				MostRecentlyQueued = UIBlockPositions.Top;
			}
			else if (MostRecentlyQueued == UIBlockPositions.Bottom && blockTypes >= 3)
			{
				// new position is right
				PositionQueue.Enqueue(UIBlockPositions.Right);
				MostRecentlyQueued = UIBlockPositions.Right;
			}
			else if (MostRecentlyQueued == UIBlockPositions.Top || (MostRecentlyQueued == UIBlockPositions.Right && blockTypes == 3) || (MostRecentlyQueued == UIBlockPositions.Bottom && blockTypes == 2) )
			{
				//new position is left
				PositionQueue.Enqueue(UIBlockPositions.Left);
				MostRecentlyQueued = UIBlockPositions.Left;
			}
			SetActiveBlock();
		}		
	}

	public void MoveBlock() {
		StartCoroutine(MovePosition());
	}

	IEnumerator MovePosition() {
		//set moving to true so no other rotations will be processed simultaneously
		Moving = true;

		//get new and old positions and scales
		UIBlockPositions newpos = PositionQueue.Dequeue();
		Vector3 newposPosition = GetLocationPositionFromPosition(newpos);
		Vector3 newposScale = GetScaleFromPosition(newpos);	

		Vector3 oldposPosition = rt.localPosition;
		Vector3 oldposScale = rt.transform.localScale;

		//set the midpoint to be an average of the two positions
		//if avg(x) > 0, then jut it out a bit to the right, avg(x) < 0 jut it out a bit to the left
		//if avg(y) > y, jut it up, avg(y) < 0, jut it down
		Vector3 center = (newposPosition + oldposPosition) * 0.5f;

		Vector3 jut = (newposPosition + oldposPosition) * .25f;
		jut.y = Mathf.Abs(jut.y) > 0.0001 ? jut.y : leftOffset.y - topOffset.y;
		center -= jut;

		//method copied from Slerp documentation
		Vector3 relStart = oldposPosition - center;
		Vector3 relEnd = newposPosition - center;
		
		float completionTime = 0.5f;
		float startTime = Time.time;	

		while(Time.time - startTime < completionTime + Time.deltaTime) {
			float percentComplete = (Time.time - startTime) / completionTime;
			this.transform.localScale = Vector3.Lerp(oldposScale, newposScale, percentComplete);

			this.transform.localPosition = Vector3.Slerp(relStart, relEnd, percentComplete);
			this.transform.localPosition += center;

			yield return new WaitForEndOfFrame();
		}

		CurrentPosition = newpos;
		//set moving to false so next movement can be processed
		Moving = false;

		yield return null;
	}

	public static Vector3 GetLocationPositionFromPosition(UIBlockPositions p) {
		switch (p)
		{
			case UIBlockPositions.Bottom:
				return bottomOffset; 
			case UIBlockPositions.Left:
				return leftOffset;
			case UIBlockPositions.Right:
				return rightOffset;
			default:
			case UIBlockPositions.Top:
				return topOffset;
		}
	}
	public static Vector3 GetScaleFromPosition(UIBlockPositions p)
	{
		if(p == UIBlockPositions.Bottom)
			return mainScale;
		else
			return secondaryScale;
	}
}
