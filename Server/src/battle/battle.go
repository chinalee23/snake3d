package battle

import (
	"config"
	"fmt"
	"network"
	"network/netdef"
)

const (
	_enterReq     = 1
	_enterRsp     = 2
	_roominfo     = 3
	_roomstartReq = 4
	_battleStart  = 5
	_ready        = 6
	_fight        = 7
	_frame        = 8
)

type stSnake struct {
	X     float32 `json:"x"`
	Y     float32 `json:"y"`
	Z     float32 `json:"z"`
	Len   float32 `json:"len"`
	Speed int     `json:"speed"`
}

type stBattleConfig struct {
	Ip     string    `json:"ip"`
	Port   int       `json:"port"`
	Snakes []stSnake `json:"snakes"`
}

var chConnection chan netdef.Connection
var currRoom *stRoom
var roomid int
var battleCfg *stBattleConfig

func eh(err error) {

}

func initBattle() {
	battleCfg = &stBattleConfig{}
	config.LoadBattle("config/battle_config", battleCfg)
	fmt.Println(battleCfg)

	chConnection = make(chan netdef.Connection, 1000)

	// svr, err := network.NewTcpServer("192.168.10.231:12345", eh)
	svr, err := network.NewTcpServer(fmt.Sprintf("%s:%d", battleCfg.Ip, battleCfg.Port), eh)
	if err != nil {
		return
	}
	svr.Start(chConnection)

	roomid = 1
	currRoom = newRoom(roomid)
	roomid++
}

func update() {
	for {
		conn := <-chConnection
		if currRoom.status == "start" {
			currRoom = newRoom(roomid)
			roomid++
		}
		currRoom.acceptConnection(conn)
	}
}

func Start() {
	fmt.Println("battle start...")
	initBattle()
	go update()
}
