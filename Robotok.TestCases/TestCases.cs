using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Moq;

namespace Robotok.TestCases
{
    [TestClass]
    public class TestCases
    {
        private RobotokGameModel _model = null!;
        private RobotokTable _mockedTable = null!;
        private Mock<IRobotokDataAccess> _mock = null!;
        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new RobotokTable(17, 28);
            _mock = new Mock<IRobotokDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
        }
        [TestMethod]
        public void GeneratedFieldsAmount()
        {
            int playingField = 0;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    playingField++;
                }
            }

            Assert.AreEqual(476, playingField);
        }

    }
}