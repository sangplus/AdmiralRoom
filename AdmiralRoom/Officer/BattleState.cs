﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class BattleState : NotifyBase
    {
        public BattleState()
        {
            Staff.API("api_port/port").Subscribe((Fiddler.Session x) =>
            {
                InSortie = false;
                CurrentMap = null;
                CurrentNode = null;
            });
            Staff.API("api_req_map/start").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_map/next").Subscribe<map_start_next>(StartNextHandler);
            Staff.API("api_req_sortie/battleresult").Subscribe<sortie_battleresult>(BattleResultHandler);
        }

        #region InSortie
        private bool _insortie;
        public bool InSortie
        {
            get { return _insortie; }
            set
            {
                if (_insortie != value)
                {
                    _insortie = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CurrentMap
        private MapInfo _currentmap;
        public MapInfo CurrentMap
        {
            get { return _currentmap; }
            set
            {
                if (_currentmap != value)
                {
                    _currentmap = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CurrentNode
        private MapNode _currentnode;
        public MapNode CurrentNode
        {
            get { return _currentnode; }
            set
            {
                if (_currentnode != value)
                {
                    _currentnode = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        void StartNextHandler(map_start_next api)
        {
            CurrentMap = Staff.Current.MasterData.MapAreas[api.api_maparea_id][api.api_mapinfo_no];
            CurrentNode = new MapNode(api);
        }
        void BattleResultHandler(sortie_battleresult api)
        {
            if (CurrentNode.Type == MapNodeType.BOSS)
            {
                StaticCounters.BossCounter.Increase();
                if (ConstData.RanksWin.Contains(api.api_win_rank))
                {
                    StaticCounters.BossWinCounter.Increase();
                    if (CurrentMap.AreaNo == 2) StaticCounters.Map2Counter.Increase();
                    else if (CurrentMap.AreaNo == 3 && CurrentMap.No >= 3) StaticCounters.Map3Counter.Increase();
                    else if (CurrentMap.AreaNo == 4) StaticCounters.Map4Counter.Increase();
                    else if (CurrentMap.Id == 15) StaticCounters.Map1_5Counter.Increase();
                }
            }
        }
    }
}
