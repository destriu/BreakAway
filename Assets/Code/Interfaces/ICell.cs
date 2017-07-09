
public interface ICell 
{
	#region Stat Properties
	float Health {get;}
	#endregion

	#region Cell Actions
	void Detect();// This should be used for any cell to detect what other cells are around them ion a certain radius
	void ConsumeCell();// This should be used if a cell is close enough to an opposing cell and is capable of consuming them
	void DestroyCell();// This should be used after a cells health has reached 0
	#endregion

}
