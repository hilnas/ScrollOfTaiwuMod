return {
	Description = " 此mod可能伤害强大 纯爱战士与未成年玩家请谨慎使用。。。<br> 太吾的妻子更容易爱慕别的npc/被爱慕 <br> 太吾的女性爱慕对象更容易爱慕别的npc/被爱慕 <br> 太吾的妻子与女性爱慕对象更容易与别人春宵/被欺侮 <br> 太吾的爱慕对象更容易与太吾以外的角色结婚 <br> 提高已婚角色的出轨概率 <br> 太吾的妻子与女性爱慕对象更容易被同地块男角色列为可爱慕对象",
	DefaultSettings = 
	{
		{
			Description = "太吾妻子与女爱慕对象被通地块男性加入可爱慕列表几率加成",
			DisplayName = "加入可爱慕列表几率",
			Key = "add_adoreReady_List",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 30
		},
		{
			Description = "太吾妻子与女爱慕对象被通地块男性发起爱慕申请几率加成",
			DisplayName = "发起爱慕追求几率",
			Key = "start_adore_apply",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 30
		},
		{
			Description = "发起爱慕申请后 申请成功率加成",
			DisplayName = "爱慕申请成功率",
			Key = "final_rate",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 45
		},
		{
			Description = "春宵额外概率加成",
			DisplayName = "春宵几率加成",
			Key = "sex_rate",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 20
		},
		{
			Description = "若上处春宵判定失败 则进行欺辱判定",
			DisplayName = "欺侮几率加成",
			Key = "rape_rate",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 8
		},
		{
			Description = "“他的孩子你不培养”",
			DisplayName = "出轨/被欺侮怀孕概率",
			Key = "pregnant_chance",
			MinValue = 0,
			MaxValue = 100,
			SettingType = "Slider",
			DefaultValue = 0
		},
		{
			Description = "打印日志 Debug用",
			DisplayName = "Print Log",
			Key = "printLog",
			MinValue = 0,
			MaxValue = 50,
			SettingType = "Toggle",
			DefaultValue = false
		}
	},
	BackendPlugins = 
	{
		[1] = "NtrYes.dll"
	},
	FileId = 2875372286,
	Cover = "Cover.png",
	FrontendPlugins = 
	{
	},
	Author = "掉色人想看梅琳娜微笑",
	Source = 1,
	Title = "苦主绘卷 NTR Yes！"
}