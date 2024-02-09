using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFactory
{
        public abstract IEnemy GetEnemy(EnemyType humanType);
}
