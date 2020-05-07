﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peixi
{
    /// <summary>
    /// 记录提案
    /// </summary>
    public struct Bill
    {
        public string name;
        public string action;
        public int card1;
        public int card2;
        public Bill(string m_name,string m_action,int m_card1,int m_card2)
        {
            name = m_name;
            action = m_action;
            card1 = m_card1;
            card2 = m_card2;
        }
    }
    /// <summary>
    /// 记录投票结果
    /// </summary>
    public struct Vote
    {
        public float negativeVote;
        public float positiveVote;
    }
    /// <summary>
    /// 服务器使用StartRound(List<Bill> m_playerBill)开启投票回合
    /// 服务器使用InvokeShowVoteResult(Vote m_vote)显示投票结果
    /// </summary>
    public class VoteState : RoundState
    {
        [SerializeField]
        GameObject voteFrame;
        [SerializeField]
        GameObject useExTicketFrame;
        [SerializeField]
        GameObject voteResultFrame;
        
        /// <summary>
        /// 投票开始
        /// </summary>
        public event Action onVoteRoundStart;
        /// <summary>
        /// 投票结束
        /// </summary>
        public event Action onVoteRoundEnd;
        /// <summary>
        /// 投票事件
        /// </summary>
        public event Action<bool> onVoteSent;
        /// <summary>
        /// 使用额外一票事件
        /// </summary>
        public event Action<bool> onUseTicket;
        /// <summary>
        /// 展示投票结果
        /// </summary>
        public event Action<Vote> onShowVoteResult;
        int voteRound = 1;

        List<Bill> playerBills = new List<Bill>();
        public List<Bill> PlayerBills
        {
            get { return playerBills; }
        }
        private void Start()
        {
            gameObject.SetActive(false);

            onRoundStarted += OnRoundStart;
            onRoundEnded += OnRoundEnd;
        }
        protected override void OnRoundStart()
        {
            //base.OnRoundStart();
            //print("开始投票阶段");
            //StartVoteRound();
        }
        protected override void OnRoundEnd()
        {
            base.OnRoundEnd();
            print("结束投票阶段，等待其他玩家");
            voteFrame.SetActive(false);
            voteResultFrame.SetActive(false);
            useExTicketFrame.SetActive(false);
        }
        /// <summary>
        /// 服务器开始投票回合
        /// </summary>
        /// <param name="m_playerBills">所有玩家的提案</param>
        public void RoundStartInvoke(List<Bill> m_playerBills)
        {
            playerBills = m_playerBills;
            StartVoteRound();
        }
        public void StartVoteRound()
        {
            voteFrame.SetActive(true);
            print("开始第" + voteRound + "轮投票");
            if (onVoteRoundStart != null)
            {
                onVoteRoundStart.Invoke();
            }
            else
            {
                Debug.LogWarning("onVoteRoundStart is empty");
            }
        }
        /// <summary>
        /// 结束投票回合
        /// </summary>
        public void EndVoteRound()
        {
            voteFrame.SetActive(false);
            voteRound += 1;
            if (voteRound > 3)
            {
                OnRoundEnd();
            }
            else
            {
                StartCoroutine(VoteInterval());
            }
        }
        public void InvokeVoteSent(bool m_vote)
        {
            //print("投下赞成票");
            if (onVoteSent != null)
            {
                var m_playerInfomation = FindObjectOfType<PlayerInformation>();
                if (m_playerInfomation.HaveExTicket)
                {
                    useExTicketFrame.SetActive(true);
                }
                onVoteSent.Invoke(m_vote);
            }      
        }
        public void InvokeUseTicket(bool m_useTicket)
        {
            print("使用额外一票");
            if (onUseTicket != null)
            {
                onUseTicket.Invoke(m_useTicket);
            }
        }
        /// <summary>
        /// 服务器显示投票结果
        /// </summary>
        /// <param name="m_vote"></param>
        public void InvokeShowVoteResult(Vote m_vote)
        {
            if (onShowVoteResult != null)
            {
                onShowVoteResult.Invoke(m_vote);
            }
        }
        public void InvokeVoteRoundEnd()
        {
            if (onVoteRoundEnd != null)
            {
                onVoteRoundEnd.Invoke();
            }
            else
            {
                Debug.LogWarning("onNotUseTicket is empty");
            }
        }
        IEnumerator RoundInterval()
        {
            yield return new WaitForSeconds(2);
            FindObjectOfType<AccountState>().RoundStartInvoke();
        }
        IEnumerator VoteInterval()
        {
            yield return new WaitForSeconds(1.5f);
            StartVoteRound();
        }
    }
}

