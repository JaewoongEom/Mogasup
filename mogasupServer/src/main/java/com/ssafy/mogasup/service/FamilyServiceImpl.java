package com.ssafy.mogasup.service;

import java.util.HashMap;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.ssafy.mogasup.dao.FamilyDao;
import com.ssafy.mogasup.dto.Family;

@Service
public class FamilyServiceImpl implements FamilyService{
	@Autowired
	FamilyDao dao;

	@Override
	public void createFamily(String name) {
		dao.createFamily(name);
	}

	@Override
	public int findFamilyId() {
		return dao.findFamilyId();
	}

	@Override
	public void createUserFamily(int user_id, int family_id) {
		dao.createUserFamily(user_id, family_id);
	}

	@Override
	public List<Family> familyList(int user_id) {
		return dao.familyList(user_id);
	}

	@Override
	public List<HashMap<String, String>> familyMemberList(int family_id) {
		return dao.familyMemberList(family_id);
	}

	@Override
	public void deleteFamily(int family_id) {
		dao.deleteFamily(family_id);
	}

	@Override
	public void deleteFamilyMember(int user_id, int family_id) {
		dao.deleteFamilyMember(user_id, family_id);
	}

	@Override
	public String familyName(int family_id) {
		return dao.familyName(family_id);
	}

	
	
}
