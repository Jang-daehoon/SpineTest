using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//Blink가 Idle도중 실행되도록 작성
namespace SpineTest
{
    public class PlayerMixedAnim : MonoBehaviour
    {
        #region public Field
        //스파인 애니메이션을 위한 것
        public SkeletonAnimation skeletonAnimation;
        public AnimationReferenceAsset[] animClip;

        //애니메이션에 대한 ENUM
        public enum AnimState
        {
            IDLE, BLINK
        }
        #endregion
        #region private Field
        //현재 애니메이션 처리가 무엇인지에 대한 변수
        private AnimState animState;
        private string currentAnimation;    //현재 어떤 애니메이션이 재생되고 있는지에 대한 변수

        //Move처리
        private Rigidbody2D rig;
        private float xx;
        private float minimumDelay = 0.15f;
        private float maximumDelay = 3f;
        #endregion

        private void Awake() => rig = GetComponent<Rigidbody2D>();

        private void Start()
        {
            //Idle상태에 눈깜빡임 모션 추가
            StartCoroutine(_AsncFaceChange(animClip[(int)AnimState.BLINK], false, 1f));
        }
        private void Update()
        {
            xx = Input.GetAxisRaw("Horizontal");

            if(xx != 0f)
            {
                transform.localScale = new Vector2(xx, 1);
            }

            //애니메이션 적용
            SetCurrentAnimation(animState);

            if (Input.GetKeyDown(KeyCode.Space)) // 예: 피격 상황
            {
                ChangeFaceOne(animClip[(int)AnimState.BLINK], false, 1f, animClip[(int)AnimState.IDLE]);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                ChangeFaceOnly(animClip[(int)AnimState.BLINK], false, 1f, 1);
            }

        }
        private void FixedUpdate() => rig.velocity = new Vector2(xx * 300 * Time.deltaTime, rig.velocity.y);

        private void _AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
        {
            if (animClip.name.Equals(currentAnimation)) return; //매 프레임마다 재실행되지 않도록 리턴.

            //해당 애니메이션으로 변경한다.
            skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
            skeletonAnimation.loop = loop;
            skeletonAnimation.timeScale = timeScale;

            //현재 재생되고 있는 애니메이션 값을 변경.
            currentAnimation = animClip.name;
        }

        //표정의 변화가 있다면
        private IEnumerator _AsncFaceChange(AnimationReferenceAsset animClip, bool loop, float timeScale, int track = 1)
        {
            if (animClip.name.Equals(currentAnimation)) yield break; //매 프레임마다 재실행되지 않도록 리턴.
           
            while(true)
            {
                skeletonAnimation.state.SetAnimation(track, animClip, loop).TimeScale = timeScale;  //1번 트랙에서 재생
                yield return new WaitForSeconds(Random.Range(minimumDelay, maximumDelay));
            }
        }
        //Stage에 따라 동작하는 애니메이션을 변경해준다.
        private void SetCurrentAnimation(AnimState _state)
        {
            switch (_state)
            {
                case AnimState.IDLE:
                    _AsncAnimation(animClip[(int)AnimState.IDLE], true, 1f);
                    break;
            }
            //짧게 개선한 코드
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }
        //일시적인 피격과 같은 상황에 표정이 바뀌려면 1회 재생 후 원래 상태로 복원해야한다.
        private void ChangeFaceOne(AnimationReferenceAsset animClip, bool loop, float timeScale, AnimationReferenceAsset defaultAnimClip, int track = 1)
        {
            // 지정된 애니메이션 재생
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, animClip, loop);
            entry.TimeScale = timeScale;

            // 애니메이션 종료 시 기본 상태로 복원
            entry.Complete += (trackEntry) =>
            {
                skeletonAnimation.state.SetAnimation(track, defaultAnimClip, true).TimeScale = 1f; // 기본 애니메이션으로 복원
            };
        }
        //표정만 변경하려면?
        private void ChangeFaceOnly(AnimationReferenceAsset faceAnimClip, bool loop, float timeScale, int track = 1)
        {
            // 지정된 트랙(1번 트랙)에서 애니메이션 재생
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, faceAnimClip, loop);
            entry.TimeScale = timeScale;
            print($"표정 변화! 변화경한 클립{faceAnimClip.name}");
            // 표정 애니메이션이 종료되면 해당 트랙을 비우거나 유지
            if (!loop)
            {
                entry.Complete += (trackEntry) =>
                {
                    skeletonAnimation.state.ClearTrack(track); // 표정 애니메이션이 끝나면 트랙을 비움
                };
            }
        }

    }
}
