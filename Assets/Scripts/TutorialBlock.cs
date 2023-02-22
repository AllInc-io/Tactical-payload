using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class TutorialBlock : MonoBehaviour
{

    [SerializeField] public Transform mustGoToPoint1;
    [SerializeField] public Transform mustGoToPoint2;
    [SerializeField] public Transform mustGoToPoint3;


    [SerializeField] Transform enemiesSpawn;
    [SerializeField] Enemy enemyToSpawnPrefab;

    public int length = 25;

    public void StartTuto()
    {
        StartCoroutine(GameplayTutorialCoroutine());
    }
    IEnumerator GameplayTutorialCoroutine()
    {


        float margin = 1f;

        R.get.levelManager.level.pauseWaves = true;
        //wait until everything is init
        yield return new WaitForSeconds(1f);


        yield return new WaitUntil(() => R.get.game.heroes.Where(hero => hero.inCinematic).Count() == 0);


        //-Un deplacement de 1 perso vers un point donn?(draw au bon endroit)
        R.get.ui.menuIngame.ShowTutorialText("Draw a path to move your Heroes");

        mustGoToPoint1.gameObject.SetActive(true);
        R.get.ui.menuIngame.SetPointer(R.get.game.heroes[0].transform.position, mustGoToPoint1.position);
        R.get.game.controls.SetForcedDraw(R.get.game.heroes[0], mustGoToPoint1.position, margin);

        yield return new WaitWhile(() => R.get.game.controls.inForcedDraw);

        R.get.game.controls.Stop();

        yield return new WaitUntil(() => Vector3.Distance(R.get.game.heroes[0].transform.position, mustGoToPoint1.transform.position) <= margin);

        mustGoToPoint1.gameObject.SetActive(false);

        //- Deplacement du second perso vers un autre point donn?e(draw au bon endroit)
        R.get.game.controls.OnStart();
        R.get.game.controls.SetForcedDraw(R.get.game.heroes[1], mustGoToPoint2.position, margin);
        mustGoToPoint2.gameObject.SetActive(true);
        R.get.ui.menuIngame.SetPointer(R.get.game.heroes[1].transform.position, mustGoToPoint2.position);

        yield return new WaitUntil(() => Vector3.Distance(R.get.game.heroes[1].transform.position, mustGoToPoint2.transform.position) <= margin);

        mustGoToPoint2.gameObject.SetActive(false);
        R.get.ui.menuIngame.HideTutorialText();

        //-Lancement de vague(on peut pas deplacer les persos ? ce moment l?)


        //R.get.game.controls.Stop();

        R.get.ui.menuIngame.ShowTutorialText("Destroy obstacles on the path");

        yield return new WaitForSeconds(3);

        R.get.ui.menuIngame.ShowTutorialText("Kill the zombies !");

        Enemy enemy = Instantiate(enemyToSpawnPrefab, enemiesSpawn);
        enemy.Init();

        //-Deplacement du perso 2 vers un autre point pendant la vague(draw vers un point) si il n'a pas boug?

        //R.get.game.controls.OnStart();

        /*
        if (Vector3.Distance(R.get.game.heroes[0].transform.position, mustGoToPoint1.position) <= margin)
        {

            Time.timeScale = 0;

            R.get.game.controls.SetForcedDraw(R.get.game.heroes[0], mustGoToPoint3.position, margin);
            mustGoToPoint3.gameObject.SetActive(true);
            R.get.ui.menuIngame.SetPointer(R.get.game.heroes[0].transform.position, mustGoToPoint3.position);

            yield return new WaitUntil(() => Vector3.Distance(R.get.game.heroes[0].transform.position, mustGoToPoint3.transform.position) <= margin);

            mustGoToPoint3.gameObject.SetActive(false);
        }
        */

        yield return new WaitForSeconds(5);

        R.get.ui.menuIngame.ShowTutorialText("Take your vehicle as far as you can");
        R.get.levelManager.level.pauseWaves = false;
        yield return new WaitForSeconds(5);

        R.get.ui.menuIngame.HideTutorialText();

    }
}
