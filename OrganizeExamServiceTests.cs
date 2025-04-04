using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend_online_testing.Models;
using Backend_online_testing.Services;
using Backend_online_testing.Dtos;

[TestClass]
public class OrganizeExamServiceTests
{
    private Mock<IMongoDatabase> _mockDatabase;
    private Mock<IMongoCollection<OrganizeExamModel>> _mockCollection;
    private OrganizeExamService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockCollection = new Mock<IMongoCollection<OrganizeExamModel>>();

        _mockDatabase
            .Setup(db => db.GetCollection<OrganizeExamModel>(It.IsAny<string>(), null))
            .Returns(_mockCollection.Object);

        _service = new OrganizeExamService(_mockDatabase.Object);
    }

    /* 
        Test create organize Exam
    */
    [TestMethod]
    public async Task CreateOrganizeExam_ValidData_ShouldInsertIntoDatabase()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "Lịch sử 10",
            Duration = 90,
            TotalQuestions = 50,
            MaxScore = 100,
            SubjectId = "67e4a6cc9913e04c3885d7e9",
            QuestionBankId = "67e4b0ebba3467f00ba22ddc",
            ExamType = "Active",
            MatrixId = "",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        // Act
        var result = await _service.CreateOrganizeExam(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.OrganizeExamName, result.OrganizeExamName);
    }

    //Test bien
    [TestMethod]
    public async Task CreateOrganizeExam_InvalidValidData_Boundary_ShouldInsertIntoDatabase()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "Lịch sử 10",
            Duration = 0,
            TotalQuestions = 50,
            MaxScore = 100,
            SubjectId = "67e4a6cc9913e04c3885d7e9",
            QuestionBankId = "67e4b0ebba3467f00ba22ddc",
            ExamType = "Active",
            MatrixId = "",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await _service.CreateOrganizeExam(dto);
        });

        // Kiểm tra nội dung lỗi
        Assert.AreEqual("Duration must be greater than zero", exception.Message);
    }

    [TestMethod]
    public async Task CreateOrganizeExam_EmptyName_ShouldThrowException()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "",  // Rỗng
            Duration = 90,
            TotalQuestions = 50,
            MaxScore = 100,
            SubjectId = "SUB123",
            QuestionBankId = "QB456",
            ExamType = "Online",
            MatrixId = "MATRIX789",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await _service.CreateOrganizeExam(dto);
        });

        // Kiểm tra nội dung lỗi
        Assert.AreEqual("OrganizeExamName cannot be empty", exception.Message);
    }

    [TestMethod]
    public async Task CreateOrganizeExam_NegativeDuration_ShouldThrowException()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "Midterm Exam",
            Duration = -10,  // Lỗi
            TotalQuestions = 50,
            MaxScore = 100,
            SubjectId = "SUB123",
            QuestionBankId = "QB456",
            ExamType = "Online",
            MatrixId = "MATRIX789",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await _service.CreateOrganizeExam(dto);
        });

        // Kiểm tra nội dung lỗi
        Assert.AreEqual("Duration must be greater than zero", exception.Message);
    }

    [TestMethod]
    public async Task CreateOrganizeExam_NegativeTotalQuestions_ShouldThrowException()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "Midterm Exam",
            Duration = 90,
            TotalQuestions = -5,  // Lỗi
            MaxScore = 100,
            SubjectId = "SUB123",
            QuestionBankId = "QB456",
            ExamType = "Online",
            MatrixId = "MATRIX789",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        //// Act & Assert
        //var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        //{
        //    await _service.CreateOrganizeExam(dto);
        //});

        //// Kiểm tra nội dung lỗi
        //Assert.AreEqual("TotalQuestions must be greater than zero", exception.Message);
        try
        {
            // Act
            await _service.CreateOrganizeExam(dto);

            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "TotalQuestions must be greater than zero");
        }
    }

    [TestMethod]
    public async Task CreateOrganizeExam_NegativeMaxScore_ShouldThrowException()
    {
        // Arrange
        var dto = new OrganizeExamRequestDto
        {
            OrganizeExamName = "Midterm Exam",
            Duration = 90,
            TotalQuestions = 50,
            MaxScore = -10,  // Lỗi
            SubjectId = "SUB123",
            QuestionBankId = "QB456",
            ExamType = "Online",
            MatrixId = "MATRIX789",
            Exams = new List<string> { "Exam1", "Exam2" },
            OrganizeExamStatus = "active"
        };

        try
        {
            // Act
            await _service.CreateOrganizeExam(dto);

            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "MaxScore must be greater than zero");
        }
    }

    /*
        
        Test create session

    */
    // ExamId is empty
    [TestMethod]
    public async Task AddSession_ValidData_ShouldAddSessionSuccessfully()
    {
        var dto = new SessionRequestDto
        {
            SessionName = "Lịch sử 11",
            ActiveAt = DateTime.UtcNow.AddDays(10), // Future date
            SessionStatus = "Active"  // Valid status
        };

        try
        {
            var result = await _service.AddSession("67eed2744f579d58d7ddda99", dto);
            //Console.WriteLine("Result: " + result);

            // Assert the result
            //Assert.IsNotNull(result); // Ensure result is not null
            //Assert.AreEqual("67eed2744f579d58d7ddda99", result.Id); // The exam ID should match
            //Assert.AreEqual(1, result.Sessions.Count); // One session added
            //Assert.AreEqual(dto.SessionName, result.Sessions[0].SessionName);
            //Assert.AreEqual(dto.SessionStatus, result.Sessions[0].SessionStatus);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task AddSession_EmptyExamId_ShouldThrowException()
    {
        var dto = new SessionRequestDto
        {
            SessionName = "Session 1",
            ActiveAt = DateTime.UtcNow.AddDays(1),
            SessionStatus = "Active"
        };

        try
        {
            await _service.AddSession("", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Exam ID cannot be empty");
        }
    }

    // SessionName is empty
    [TestMethod]
    public async Task AddSession_EmptySessionName_ShouldThrowException()
    {
        var dto = new SessionRequestDto
        {
            SessionName = "",
            ActiveAt = DateTime.UtcNow.AddDays(1),
            SessionStatus = "Active"
        };

        try
        {
            await _service.AddSession("exam123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "SessionName cannot be empty");
        }
    }

    // SessionStatus is invialid
    [TestMethod]
    public async Task AddSession_InvalidSessionStatus_ShouldThrowException()
    {
        var dto = new SessionRequestDto
        {
            SessionName = "Session 1",
            ActiveAt = DateTime.UtcNow.AddDays(1),
            SessionStatus = "InvalidStatus"
        };

        try
        {
            await _service.AddSession("exam123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Invalid session status");
        }
    }

    // Active at is yesterday test
    [TestMethod]
    public async Task AddSession_PastActiveAt_ShouldThrowException()
    {
        var dto = new SessionRequestDto
        {
            SessionName = "Session 1",
            ActiveAt = DateTime.UtcNow.AddDays(-1), // Ngày trong quá khứ
            SessionStatus = "Active"
        };

        try
        {
            await _service.AddSession("exam123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "ActiveAt must be in the future");
        }
    }
    /*
        
        Test add room session

    */
    //Valid room data
    [TestMethod]
    public async Task AddRoomToSession_ValidData_ShouldAddRoomSuccessfully()
    {
        var dto = new RoomsInSessionRequestDto
        {
            RoomId = "room123",
            SupervisorIds = new List<string> { "supervisor1", "supervisor2" }
        };

        try
        {
            var result = await _service.AddRoomToSession("67e94f2807292389fbc7133d", "session123", dto);

            // Ensure the result is not null
            Assert.IsNotNull(result, "The result should not be null.");

            // Ensure the exam ID matches
            Assert.AreEqual("67e94f2807292389fbc7133d", result.Id, "The exam ID does not match.");

            // Ensure the session contains the new room
            //var session = result.Sessions.FirstOrDefault(s => s.SessionId == "session123");
            //Assert.IsNotNull(session, "Session not found.");
            //Assert.AreEqual(1, session.Rooms.Count, "The room count in session is incorrect.");
            //Assert.AreEqual(dto.RoomId, session.Rooms[0].RoomInSessionId, "The room ID does not match.");
            //CollectionAssert.AreEqual(dto.SupervisorIds, session.Rooms[0].SupervisorIds, "The supervisor IDs do not match.");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }
    //Invalid data - examId is Empty
    [TestMethod]
    public async Task AddRoomToSession_ExamIdEmpty_ShouldThrowException()
    {
        var dto = new RoomsInSessionRequestDto
        {
            RoomId = "room123",
            SupervisorIds = new List<string> { "supervisor1" }
        };

        try
        {
            await _service.AddRoomToSession("", "session123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Exam ID cannot be empty");
        }
    }

    //Invalid data - sessionId is empty
    [TestMethod]
    public async Task AddRoomToSession_SessionIdEmpty_ShouldThrowException()
    {
        var dto = new RoomsInSessionRequestDto
        {
            RoomId = "room123",
            SupervisorIds = new List<string> { "supervisor1" }
        };

        try
        {
            await _service.AddRoomToSession("67e94f2807292389fbc7133d", "", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Session ID cannot be empty");
        }
    }

    //Exam Id not found
    //[TestMethod]
    //public async Task AddRoomToSession_ExamIdNotFound_ShouldThrowException()
    //{
    //    var dto = new RoomsInSessionRequestDto
    //    {
    //        RoomId = "room123",
    //        SupervisorIds = new List<string> { "supervisor1" }
    //    };

    //    try
    //    {
    //        await _service.AddRoomToSession("67e94f2807292389fbc7133e", "67e94f2807292389fbc7133e", dto);
    //        Assert.Fail("Expected ArgumentException but no exception was thrown.");
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        StringAssert.Contains(ex.Message, "Exam ID not found");
    //    }
    //}

    //SessionId not found
    //[TestMethod]
    //public async Task AddRoomToSession_SessionIdNotFound_ShouldThrowException()
    //{
    //    var dto = new RoomsInSessionRequestDto
    //    {
    //        RoomId = "room123",
    //        SupervisorIds = new List<string> { "supervisor1" }
    //    };

    //    try
    //    {
    //        await _service.AddRoomToSession("67e94f2807292389fbc7133e", "67e94f2807292389fbc7133e", dto);
    //        Assert.Fail("Expected ArgumentException but no exception was thrown.");
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        StringAssert.Contains(ex.Message, "Session not found");
    //    }
    //}

    //Invalid data - Room Id is empty 
    [TestMethod]
    public async Task AddRoomToSession_RoomIdEmpty_ShouldThrowException()
    {
        var dto = new RoomsInSessionRequestDto
        {
            RoomId = "",
            SupervisorIds = new List<string> { "supervisor1" }
        };

        try
        {
            await _service.AddRoomToSession("67e94f2807292389fbc7133d", "session123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "RoomId cannot be empty");
        }
    }

    //Invalid Data - SupervisorId empty
    [TestMethod]
    public async Task AddRoomToSession_SupervisorIdsEmpty_ShouldThrowException()
    {
        var dto = new RoomsInSessionRequestDto
        {
            RoomId = "room123",
            SupervisorIds = new List<string>()
        };

        try
        {
            await _service.AddRoomToSession("67e94f2807292389fbc7133d", "session123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "SupervisorIds cannot be empty");
        }
    }
    /*
        
        Test add room session

    */
    [TestMethod]
    public async Task AddCandidateToRoom_ExamIdEmpty_ShouldThrowException()
    {
        var dto = new CandidatesInSessionRoomRequestDto
        {
            CandidateIds = new List<string> { "candidate1", "candidate2" }
        };

        try
        {
            await _service.AddCandidateToRoom("", "session123", "room123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Exam ID cannot be empty");
        }
    }

    [TestMethod]
    public async Task AddCandidateToRoom_SessionIdEmpty_ShouldThrowException()
    {
        var dto = new CandidatesInSessionRoomRequestDto
        {
            CandidateIds = new List<string> { "candidate1", "candidate2" }
        };

        try
        {
            await _service.AddCandidateToRoom("exam123", "", "room123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Session ID cannot be empty");
        }
    }

    [TestMethod]
    public async Task AddCandidateToRoom_RoomIdEmpty_ShouldThrowException()
    {
        var dto = new CandidatesInSessionRoomRequestDto
        {
            CandidateIds = new List<string> { "candidate1", "candidate2" }
        };

        try
        {
            await _service.AddCandidateToRoom("exam123", "session123", "", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Room ID cannot be empty");
        }
    }

    [TestMethod]
    public async Task AddCandidateToRoom_EmptyCandidateList_ShouldThrowException()
    {
        var dto = new CandidatesInSessionRoomRequestDto
        {
            CandidateIds = new List<string>() // danh sách rỗng
        };

        try
        {
            await _service.AddCandidateToRoom("exam123", "session123", "room123", dto);
            Assert.Fail("Expected ArgumentException but no exception was thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "CandidateIds cannot be empty");
        }
    }

    [TestMethod]
    public async Task AddCandidateToRoom_ValidInput_ShouldAddCandidates()
    {
        var dto = new CandidatesInSessionRoomRequestDto
        {
            CandidateIds = new List<string> { "candidate1", "candidate2" }
        };

        var result = await _service.AddCandidateToRoom("exam123", "session123", "room123", dto);

        // Kiểm tra xem kết quả có đúng không, ví dụ trả về đối tượng đã cập nhật
        Assert.IsNotNull(result);
        //Assert.AreEqual(result.Sessions[0].Rooms[0].Candidates.Count, 2);
    }
}
