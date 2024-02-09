using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    public IEnemy GetEnemy(EnemyType type,GameManagerScript.DifficultyState difficulty)
    {
        
        //switch (type)
        //{
        //    case EnemyType.Red:
        //      spawnObject();
        //    IEnemy Red = new Red();
        //    return Red;

        //    //case EnemyType.Blue:
        //     //IEnemy Blue = new Blue();
        //     //return Blue;

        //    //case EnemyType.Green:
        //     //IEnemy Green = new Green();
        //     //return Green;
        //}
        GameObject enemyObject = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        IEnemy enemy = enemyObject.GetComponent<IEnemy>();
        if (enemy != null)
        {
            enemy.SetDifficulty(difficulty);
        }
        return enemy;

        //return null;
    }

    //this can be removed

    //void spawnObject()
    //{
    //    Instantiate(enemyPrefab, transform.position, Quaternion.identity); 
    //}

}


