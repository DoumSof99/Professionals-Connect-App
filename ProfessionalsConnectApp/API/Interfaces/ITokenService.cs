﻿using API.Entities;

namespace API.Interfaces {
    public interface ITokenService {

        string GreateToken(AppUser user);
    }
}
